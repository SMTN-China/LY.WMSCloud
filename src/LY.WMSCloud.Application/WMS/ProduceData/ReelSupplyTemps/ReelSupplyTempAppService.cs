using Abp.Configuration;
using LY.WMSCloud.Base;
using LY.WMSCloud.CommonService;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto;
using LY.WMSCloud.WMS.ProduceData.ReelSupplyTemps.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReelSupplyTemps
{
    public class ReelSupplyTempAppService : ServiceBase<ReelSupplyTemp, ReelSupplyTempDto, string>, IReelSupplyTempAppService
    {
        readonly IWMSRepositories<ReelSupplyTemp, string> _repository;
        readonly IWMSRepositories<ReadyMBill, string> _repositoryrb;
        readonly IWMSRepositories<ReadyMBillDetailed, string> _repositoryrbd;
        readonly IWMSRepositories<Reel, string> _repositoryReel;
        readonly IWMSRepositories<StorageLocation, string> _repositorySL;
        readonly LightService LightService;
        readonly IWMSRepositories<MPN, string> _repositorympn;
        readonly IWMSRepositories<ReelMoveMethod, string> _repositoryRMM;

        public ReelSupplyTempAppService(
            IWMSRepositories<ReelSupplyTemp, string> repository,
            IWMSRepositories<ReadyMBill, string> repositoryrb,
            IWMSRepositories<ReadyMBillDetailed, string> repositoryrbd,
            IWMSRepositories<Reel, string> repositoryReel,
            IWMSRepositories<StorageLocation, string> repositorySL,
            LightService lightService,
            IWMSRepositories<ReelMoveMethod, string> repositoryRMM,
            IWMSRepositories<MPN, string> repositorympn
            ) : base(repository)
        {
            _repository = repository;
            _repositoryrb = repositoryrb;
            _repositoryrbd = repositoryrbd;
            _repositoryRMM = repositoryRMM;
            _repositoryReel = repositoryReel;
            _repositorympn = repositorympn;
            _repositorySL = repositorySL;
            LightService = lightService;
        }

        public async Task<ReelSupplyResultDto> Supply(ReelSupplyInputDto[] input)
        {
            ReelSupplyResultDto res = new ReelSupplyResultDto() { ErrorMsgs = new List<string>(), IsSuccess = true };

            List<ReelSupplyTemp> reelSupplyTemps = new List<ReelSupplyTemp>();

            await _repository.DeleteAsync(r => true);

            var la = await _repositorySL.GetAllListAsync(s => s.LightState != LightState.Off);
            foreach (var sl in la)
            {
                sl.LightState = LightState.Off;
                // await _repositorySL.UpdateAsync(sl);
            }
            CurrentUnitOfWork.SaveChanges();
            // 查询备损数量
            var readyLossQty = SettingManager.GetSettingValueForTenant<int>("readyLossQty", AbpSession.TenantId.Value);//  int.Parse(_repositoryT.FirstOrDefault(c => c.TenantId == AbpSession.TenantId && c.Name == "readyLossQty").Value);

            // 查询提前预警天数
            var overdueDay = SettingManager.GetSettingValueForTenant<int>("overdueDay", AbpSession.TenantId.Value); //int.Parse(_repositoryT.FirstOrDefault(c => c.TenantId == AbpSession.TenantId && c.Name == "overdueDay").Value);

            // 查询强制先进先出天数
            var mustFifoDay = SettingManager.GetSettingValueForTenant<int>("mustFifoDay", AbpSession.TenantId.Value); //int.Parse(_repositoryT.FirstOrDefault(c => c.TenantId == AbpSession.TenantId && c.Name == "mustFifoDay").Value);

            // 补料调拨策略
            var reelSupplyMethodId = SettingManager.GetSettingValueForTenant("reelSupplyMethodId", AbpSession.TenantId.Value);// _repositoryT.FirstOrDefault(c => c.TenantId == AbpSession.TenantId && c.Name == "reelSupplyMethodId").Value;

            // 获取补料调拨策略
            var reelSupplyMethod = await _repositoryRMM.GetAllIncluding(r => r.OutStorages).FirstOrDefaultAsync(r => r.Id == reelSupplyMethodId);

            if (reelSupplyMethod == null)
            {
                res.ErrorMsgs.Add("未配置补料调拨策略");
                throw new LYException(res);
            }


            // 按备料单分组
            foreach (var item in input.GroupBy(r => r.ReadyMBillId))
            {
                // 查询当前备料单详细
                var readyNow = await _repositoryrb.FirstOrDefaultAsync(item.Key);

                if (readyNow == null)
                {
                    res.ErrorMsgs.Add("备料单[" + item.Key + "]不存在");

                    continue;
                }

                // 先假设没有关联的备料单信息
                var allReadys = new List<ReadyMBill>() { readyNow };

                if (readyNow.ReReadyMBillId != null)
                {
                    // 查询备料单关联的所有备料单是否包含当前物料
                    allReadys = await _repositoryrb.GetAll().Where(r => r.ReReadyMBillId == readyNow.ReReadyMBillId).ToListAsync();
                }

                foreach (var pn in item)
                {
                    // 查询所有备料详细是否包含该物料
                    var pnInfo = await _repositoryrbd.GetAll().Where(r => allReadys.Select(rm => rm.Id).Contains(r.ReadyMBillId) && r.PartNoId == pn.PartNoId).FirstOrDefaultAsync();

                    if (pnInfo == null)
                    {
                        res.ErrorMsgs.Add("备料单[" + item.Key + "]及其合并的备料单不包含物料[" + pn + "]的备料信息");
                        continue;
                    }

                    // 查询物料基础信息

                    var mpn = await _repositorympn.FirstOrDefaultAsync(pn.PartNoId);

                    // 查询库存中该料号所有物料 在库存中挑料,分为两部分.一部分强制先进先出。一部分按数量挑选
                    var reels = _repositoryReel.GetAll().Where(r =>
                      r.StorageLocationId.Length > 0            // 有库位
                      && r.IsActive     // 有效
                      && r.PartNoId == pn.PartNoId   // 料号正确
                      && reelSupplyMethod.OutStorages.Select(s => s.StorageId).Contains(r.StorageId)  // 仓库正确  
                      ).ToList()
                      .Where(r => r.MakeDate.AddDays(mpn.ShelfLife + r.ExtendShelfLife - overdueDay) > DateTime.Now && reelSupplyTemps.FirstOrDefault(s => s.Id == r.Id) == null) // 未过期 且未被挑料
                      .ToList(); // 按d/c 进行先进先出排序

                    // && r.MakeDate.AddDays(mpn.ShelfLife + r.ExtendShelfLife - mustFifoDay) > DateTime.Now  // 必须先进先出
                    var nowDemandSendQty = pn.Qtys;


                    // 死循环挑料,库存料盘还有。且没挑够，且站位未发够
                    while (reels.Count > 0 && nowDemandSendQty > 0)
                    {
                        // 先挑选强制发料物料
                        var reel = reels.Where(r => r.MakeDate.AddDays(mpn.ShelfLife + r.ExtendShelfLife - mustFifoDay) > DateTime.Now).OrderBy(r => r.MakeDate).FirstOrDefault();

                        if (reel == null) // 没有强制先进先出
                        {
                            // 获取库存最大料盘数量
                            var maxQtyReel = reels.OrderBy(r => r.Qty).FirstOrDefault().Qty;

                            if (nowDemandSendQty > maxQtyReel)
                            {
                                // 当需求数大于库存最大料盘数量,按FIFO取同数量中物料
                                reel = reels.Where(r => r.Qty == maxQtyReel).OrderBy(r => r.MakeDate).FirstOrDefault();
                            }
                            else
                            {
                                // 当需求数小于库存最大料盘数量
                                //  1、先查询库存数量大于需求数且差距最小物料
                                reel = reels.Where(r => r.Qty > nowDemandSendQty).OrderBy(r => reel.Qty).FirstOrDefault();

                                if (reel == null)
                                {
                                    // 2、如果没有数量大于需求数物料，直接从最大包装开始拿料
                                    reel = reels.OrderByDescending(r => r.Qty).FirstOrDefault();
                                }
                            }
                        }

                        // 模拟添加发料临时表,且标记为已发
                        reelSupplyTemps.Add(new ReelSupplyTemp()
                        {
                            Id = reel.Id,
                            IsActive = true,
                            PartNoId = pn.PartNoId,
                            DemandQty = pn.Qtys,
                            DemandSendQty = pn.Qtys,
                            IsSend = false,
                            Qty = reel.Qty,
                            SendQty = reel.Qty,
                            StorageLocationId = reel.StorageLocationId,
                            ReelMoveMethodId = reelSupplyMethodId,
                            ReReadyMBillId = readyNow.ReReadyMBillId == null ? readyNow.Id : readyNow.ReReadyMBillId,
                            ReadyMBillDetailedId = pnInfo.Id,
                            // SlotId = slot.Id
                        });

                        // 标记亮灯,真正的亮灯操作。移到专门的亮灯客户端
                        var sl = _repositorySL.Get(reel.StorageLocationId);
                        sl.LightState = LightState.On;
                        _repositorySL.Update(sl);



                        // 已发数量
                        // readySlotD.SendQty += reel.Qty;
                        // reel.SlotId = slot.Id;

                        // 更新站位数量
                        // slotQty -= reel.Qty;

                        // 移除当前料盘,让其不进入下一次挑料
                        reels.Remove(reel);

                        // 重新计算当前需求数
                        nowDemandSendQty = nowDemandSendQty - reel.Qty;
                    }
                }
            }

            // 最后进行亮灯

            foreach (var reelSupplyTemp in reelSupplyTemps)
            {
                await _repository.InsertAsync(reelSupplyTemp);
            }

            CurrentUnitOfWork.SaveChanges();

            // 亮灯
            var lights = _repositorySL.GetAllList(s => s.LightState == LightState.On);

            //小灯
            var simlights = lights.Select(l => new StorageLight() { ContinuedTime = 10, LightOrder = 1, MainBoardId = l.MainBoardId, RackPositionId = l.PositionId }).ToList();

            LightService.LightOrder(simlights);

            // 灯塔
            var houselights = simlights.Select(l => new HouseLight() { HouseLightSide = 1, LightOrder = 1, MainBoardId = l.MainBoardId }).Distinct().ToList();
            // await _notificationService.SendNotification("HouseOrder", houselights);
            LightService.HouseOrder(houselights);

            res.ErrorMsgs.Add("补料挑料成功" + (res.ErrorMsgs.Count > 0 ? "部分条目补料失败" : ""));
            res.IsSuccess = true;

            return res;
        }

        public async Task<ICollection<string>> GetPartNoIdsByKeyName(string readyBill, string keyName)
        {
            if (keyName == null)
            {
                keyName = "";
            }
            var res = await _repositoryrbd.GetAll().Where(c => c.ReadyMBillId == readyBill && c.PartNoId.Contains(keyName)).Select(r => r.PartNoId).Take(10).ToListAsync();
            return res;
        }

        public async Task<ICollection<ReadyMBillDto>> GetReadyMbillsByKeyName(string keyName)
        {
            if (keyName == null)
            {
                keyName = "";
            }
            var res = await _repositoryrb.GetAll().Where(c => c.Id.Contains(keyName)).Take(10).ToListAsync();

            return ObjectMapper.Map<List<ReadyMBillDto>>(res);
        }
    }
}
