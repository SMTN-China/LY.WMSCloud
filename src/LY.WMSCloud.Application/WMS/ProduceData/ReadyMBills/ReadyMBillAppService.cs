using Abp.Configuration;
using Abp.Dependency;
using AutoMapper;
using LY.WMSCloud.Base;
using LY.WMSCloud.CommonService;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.ReadyMBills
{
    public class ReadyMBillAppService : ServiceBase<ReadyMBill, ReadyMBillDto, string>, IReadyMBillAppService
    {

        readonly IWMSRepositories<ReadyMBill, string> _repository;
        readonly IWMSRepositories<ReadyMBillDetailed, string> _repositoryReadyMBilld;
        readonly IWMSRepositories<ReelMoveMethod, string> _repositoryRMM;
        readonly IWMSRepositories<WorkBill, string> _repositoryWorkBill;
        readonly IWMSRepositories<ReadyMBillWorkBillMap, string> _readyMBillWorkBillMap;
        readonly IWMSRepositories<Reel, string> _repositoryReel;
        readonly LightService LightService;
        readonly IWMSRepositories<ReelMoveLog, string> _reelMoveLog;
        readonly IWMSRepositories<MPN, string> _repositoryMPN;
        readonly IWMSRepositories<Line, string> _repositoryLine;
        //readonly IWMSRepositories<ReadySlot, string> _repositoryReadySlot;
        readonly IWMSRepositories<UPH, string> _repositoryUPH;
        readonly IWMSRepositories<Slot, string> _repositorySlot;
        readonly IWMSRepositories<StorageLocation, string> _repositorySL;
        readonly IWMSRepositories<ReelSendTemp, string> _repositoryRST;
        readonly IWMSRepositories<ReelShortTemp, string> _repositoryRSHT;
        readonly IWMSRepositories<BOM, string> _repositoryBOM;

        public ReadyMBillAppService(
            IWMSRepositories<ReadyMBill, string> repositoryReadyMBill,
            IWMSRepositories<ReadyMBillDetailed, string> repositoryReadyMBilld,
            IWMSRepositories<Reel, string> repositoryReel,
            IWMSRepositories<ReelMoveMethod, string> repositoryRMM,
            IWMSRepositories<WorkBill, string> repositoryWorkBill,
            IWMSRepositories<ReelSendTemp, string> repositoryRST,
            IWMSRepositories<Slot, string> repositorySlot,
            IWMSRepositories<UPH, string> repositoryUPH,
            IWMSRepositories<ReelShortTemp, string> repositoryRSHT,
            IWMSRepositories<StorageLocation, string> repositorySL,
            IWMSRepositories<MPN, string> repositoryMPN,
            IWMSRepositories<Line, string> repositoryLine,
            IWMSRepositories<BOM, string> repositoryBOM,
            IWMSRepositories<ReelMoveLog, string> reelMoveLog,
             LightService lightService,
            //IWMSRepositories<ReadySlot, string> repositoryReadySlot,
            IWMSRepositories<ReadyMBillWorkBillMap, string> readyMBillWorkBillMap) : base(repositoryReadyMBill)
        {
            this._repository = repositoryReadyMBill;
            _repositoryWorkBill = repositoryWorkBill;
            _readyMBillWorkBillMap = readyMBillWorkBillMap;
            _repositoryRMM = repositoryRMM;
            _repositoryReadyMBilld = repositoryReadyMBilld;
            _repositoryReel = repositoryReel;
            _repositoryRST = repositoryRST;
            _repositoryRSHT = repositoryRSHT;
            _repositorySL = repositorySL;
            _repositoryLine = repositoryLine;
            _repositoryMPN = repositoryMPN;
            _repositorySlot = repositorySlot;
            _repositoryUPH = repositoryUPH;
            _reelMoveLog = reelMoveLog;
            LightService = lightService;
            // _repositoryReadySlot = repositoryReadySlot;
            _repositoryBOM = repositoryBOM;
        }

        public async override Task<ReadyMBillDto> Create(ReadyMBillDto input)
        {
            input.Productstr = string.Join('|', input.WorkBills.Select(r => r.ProductId).Distinct().ToList());
            input.WorkBilQtys = string.Join('|', input.WorkBills.Select(r => r.WorkBillId + ":" + r.Qty).Distinct().ToList());
            input.Linestr = string.Join('|', input.WorkBills.Select(r => r.LineId).Distinct().ToList());

            if (input.ReelMoveMethodId == null || input.ReelMoveMethodId == "")
            {
                input.ReelMoveMethodId = await SettingManager.GetSettingValueForTenantAsync("reelMoveMethodId", AbpSession.TenantId.Value);
            }

            switch (input.MakeDetailsType)
            {
                case MakeDetailsType.BOM:
                    // 按BOM生成备料明细
                    input.ReadyMBillDetailed.Clear();
                    foreach (var wo in input.WorkBills)
                    {
                        var boms = await _repositoryBOM.GetAll().Where(r => r.ProductId == wo.ProductId).ToListAsync();
                        foreach (var bom in boms)
                        {
                            input.ReadyMBillDetailed.Add(new ReadyMBillDetailedDto()
                            {
                                BOMId = bom.Id,
                                ReelMoveMethodId = input.ReelMoveMethodId,
                                IsActive = true,
                                PartNoId = bom.PartNoId,
                                Qty = bom.Qty * wo.Qty,
                                TenantId = AbpSession.TenantId,
                            });
                        }
                    }
                    break;
                case MakeDetailsType.Slot:
                    // 按料站表生成备料明细
                    input.ReadyMBillDetailed.Clear();
                    foreach (var wo in input.WorkBills)
                    {
                        var slots = await _repositorySlot.GetAll().Where(r => r.ProductId == wo.ProductId).ToListAsync();
                        foreach (var slot in slots)
                        {
                            input.ReadyMBillDetailed.Add(new ReadyMBillDetailedDto()
                            {
                                SlotId = slot.Id,
                                ReelMoveMethodId = input.ReelMoveMethodId,
                                IsActive = true,
                                PartNoId = slot.PartNoId,
                                Qty = slot.Qty * wo.Qty,
                                TenantId = AbpSession.TenantId,
                            });
                        }
                    }
                    break;
                case MakeDetailsType.Detailed:
                    // 直接传入备料明细灯,不做任何操作
                    break;
                default:
                    break;
            }
            var res = await base.Create(input);

            return res;
        }

        public async override Task<ReadyMBillDto> Update(ReadyMBillDto input)
        {
            input.Productstr = string.Join('|', input.WorkBills.Select(r => r.ProductId).Distinct().ToList());
            input.WorkBilQtys = string.Join('|', input.WorkBills.Select(r => r.WorkBillId + ":" + r.Qty).Distinct().ToList());
            input.Linestr = string.Join('|', input.WorkBills.Select(r => r.LineId).Distinct().ToList());
            if (input.ReelMoveMethodId == null || input.ReelMoveMethodId == "")
            {
                // var readyLossQty = SettingManager.GetSettingValueForTenant("reelMoveMethodId", AbpSession.TenantId.Value); //(await _repositoryT.FirstOrDefaultAsync(c => c.TenantId == AbpSession.TenantId && c.Name == "reelMoveMethodId")).Value;
                input.ReelMoveMethodId = SettingManager.GetSettingValueForTenant("reelMoveMethodId", AbpSession.TenantId.Value);
            }
            // 删除工单明细
            foreach (var item in _readyMBillWorkBillMap.GetAll().Where(r => r.ReadyMBillId == input.Id).ToList())
            {
                _readyMBillWorkBillMap.Delete(item.Id);
            }

            // 删除备料单明细
            var rms = _repositoryReadyMBilld.GetAllList(rm => rm.ReadyMBillId == input.Id);
            foreach (var item in rms)
            {
                _repositoryReadyMBilld.Delete(item.Id);
            }

            CurrentUnitOfWork.SaveChanges();
            switch (input.MakeDetailsType)
            {
                case MakeDetailsType.BOM:
                    // 按BOM生成备料明细
                    input.ReadyMBillDetailed.Clear();
                    foreach (var wo in input.WorkBills)
                    {
                        var boms = await _repositoryBOM.GetAll().Where(r => r.ProductId == wo.ProductId).ToListAsync();
                        foreach (var bom in boms)
                        {
                            input.ReadyMBillDetailed.Add(new ReadyMBillDetailedDto()
                            {
                                BOMId = bom.Id,
                                ReelMoveMethodId = input.ReelMoveMethodId,
                                IsActive = true,
                                PartNoId = bom.PartNoId,
                                Qty = bom.Qty * wo.Qty,
                                TenantId = AbpSession.TenantId,
                            });
                        }
                    }
                    break;
                case MakeDetailsType.Slot:
                    // 按料站表生成备料明细
                    input.ReadyMBillDetailed.Clear();
                    foreach (var wo in input.WorkBills)
                    {
                        var slots = await _repositorySlot.GetAll().Where(r => r.ProductId == wo.ProductId).ToListAsync();
                        foreach (var slot in slots)
                        {
                            input.ReadyMBillDetailed.Add(new ReadyMBillDetailedDto()
                            {
                                SlotId = slot.Id,
                                ReelMoveMethodId = input.ReelMoveMethodId,
                                IsActive = true,
                                PartNoId = slot.PartNoId,
                                Qty = slot.Qty * wo.Qty,
                                TenantId = AbpSession.TenantId,
                            });
                        }
                    }
                    break;
                case MakeDetailsType.Detailed:
                    // 直接传入备料明细灯,不做任何操作
                    break;
                default:
                    break;
            }
            var res = await base.Update(input);
            return res;
        }

        public async Task<ICollection<ReadyMBillDetailedDto>> GetDetailedById(string id)
        {
            var res = await _repositoryReadyMBilld.GetAll().Where(r => r.ReadyMBillId == id).ToListAsync();

            return ObjectMapper.Map<List<ReadyMBillDetailedDto>>(res);
        }

        public async Task<ICollection<ReadyMBillWorkBillMapDto>> GetWorkBillById(string id)
        {
            var res = await _readyMBillWorkBillMap.GetAll().Where(r => r.ReadyMBillId == id).ToListAsync();

            return ObjectMapper.Map<List<ReadyMBillWorkBillMapDto>>(res);
        }

        public async Task<ICollection<ReadyMBillDto>> GetFollowReadyMBillKeyName(string keyName)
        {
            var res = await _repository.GetAll().Where(r => r.Id.Contains(keyName)).Take(10).ToListAsync(); ;

            return ObjectMapper.Map<List<ReadyMBillDto>>(res);
        }

        public async Task<bool> BatchInsOrUpdate(ICollection<RBBatchReadyMBillDto> input)
        {
            try
            {
                // 汇总
                var grouping = input.GroupBy(i => new { i.ProductId, i.Id, i.WorkBillId, i.WorkQty, i.LineId, i.IsActive, i.ForCustomerMStorageId, i.ForSelfMStorageId, i.ReelMoveMethodId });
                foreach (var item in grouping)
                {
                    // 检查线别
                    var line = await _repositoryLine.FirstOrDefaultAsync(item.Key.LineId);
                    if (line == null)
                    {
                        await _repositoryLine.InsertAsync(new Line()
                        {
                            Id = item.Key.LineId,
                            Name = item.Key.LineId,
                            IsActive = true,
                            ForCustomerMStorageId = item.Key.ForCustomerMStorageId,
                            ForSelfMStorageId = item.Key.ForSelfMStorageId,
                            Remark = "系统自动维护,相关仓库可自己改动"
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                    // 检查工单
                    var workBill = await _repositoryWorkBill.FirstOrDefaultAsync(item.Key.WorkBillId);
                    if (workBill == null)
                    {
                        await _repositoryWorkBill.InsertAsync(new WorkBill()
                        {
                            Id = item.Key.WorkBillId,
                            WorkBillStatus = WorkBillStatus.New,
                            LineId = item.Key.LineId,
                            ProductId = item.Key.ProductId,
                            Qty = item.Key.WorkQty,
                            IsActive = true,
                            Remark = "自动同步产生工单"
                        });
                    }
                    else
                    {
                        workBill.LineId = item.Key.LineId;
                        workBill.ProductId = item.Key.ProductId;
                        workBill.Qty = item.Key.WorkQty;
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                    // 检查备料单
                    var readyMBill = await _repository.FirstOrDefaultAsync(item.Key.Id);
                    if (workBill == null)
                    {
                        await _repository.InsertAsync(new ReadyMBill()
                        {
                            Id = item.Key.Id,
                            IsActive = true,
                            MakeDetailsType = MakeDetailsType.Slot,
                            ReadyMType = ReadyMType.JIT,
                            ReelMoveMethodId = item.Key.ReelMoveMethodId,
                            WorkBills = new List<ReadyMBillWorkBillMap>()
                        {
                             new ReadyMBillWorkBillMap()
                             {
                                  Qty=item.Key.WorkQty,
                                  WorkBillId=item.Key.WorkBillId,
                                  ReadyMBillId=item.Key.Id
                             }
                        },
                            Remark = "自动同步备料单",
                            Productstr = item.Key.ProductId,
                            Linestr = item.Key.LineId,
                            WorkBilQtys = item.Key.Id + ':' + item.Key.WorkQty
                        });
                    }
                    else
                    {
                        var wrMap = await _readyMBillWorkBillMap.GetAll().Where(r => r.WorkBillId == item.Key.WorkBillId && r.ReadyMBillId == item.Key.Id).FirstOrDefaultAsync();

                        if (wrMap != null)
                        {
                            wrMap.Qty = item.Key.WorkQty;
                        }

                        readyMBill.Productstr = item.Key.ProductId;
                        readyMBill.Linestr = item.Key.LineId;
                        readyMBill.WorkBilQtys = item.Key.Id + ':' + item.Key.WorkQty;
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                    // 检查备料单详细
                    foreach (var partNo in item)
                    {
                        // 检查是否存在
                        var readyMBillDetailed = await _repositoryReadyMBilld.FirstOrDefaultAsync(r => r.ReadyMBillId == item.Key.Id && r.PartNoId == partNo.PartNoId);
                        if (readyMBillDetailed == null)
                        {
                            await _repositoryReadyMBilld.InsertAsync(new ReadyMBillDetailed()
                            {
                                ReadyMBillId = partNo.Id,
                                PartNoId = partNo.PartNoId,
                                Qty = partNo.PartNoQty,
                                IsActive = true,
                                ReelMoveMethodId = partNo.ReelMoveMethodId,
                            });
                        }
                        else
                        {
                            readyMBillDetailed.Qty = partNo.PartNoQty;
                        }
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception es)
            {
                throw new LYException(es);

            }
        }

        public async Task<ReadyMResultDto> ReadyM(ReadyMDto readyM)
        {
            if (readyM.ReadyMBills.GroupBy(s => new { s.Linestr }).Count() > 1)
            {
                throw new LYException("合并工单必须为相同线别");
            }

            // 判断本次备料单是否全部有备料记录
            var rms = await _repository.GetAll().Where(r => readyM.ReadyMBills.Select(rm => rm.Id).Contains(r.Id) && r.ReReadyMBillId != null).Select(r => r.ReReadyMBillId).Distinct().ToListAsync();

            // 有过备料记录的
            if (rms.Count > 1)
            {
                // 本次备料勾选混乱
                throw new LYException("若本次备料包含多个备料单,那么本次备料必须和历史备料勾选一致");
            }
            else if (rms.Count == 1)
            {
                if (rms[0] != readyM.ReReadyMBill)
                {
                    throw new LYException("本次备料记账备料单[" + readyM.ReReadyMBill + "]和历史记账备料单[" + rms[0] + "]不一致");
                }
            }

            // 备料返回信息
            ReadyMResultDto readyMResultDto = new ReadyMResultDto() { ReReadyMBillId = readyM.ReReadyMBill };
            //// 发料单信息

            //var settinglightType = await _repositoryT.FirstOrDefaultAsync(c => c.TenantId == AbpSession.TenantId && c.Name == "lightIsRGB");
            var lightType = SettingManager.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.TenantId.Value); // settinglightType == null ? 0 : int.Parse(settinglightType.Value);
            var lightColor = LightColor.Default;

            var rst1 = _repositoryRST.FirstOrDefault(r => r.ReReadyMBillId == readyM.ReReadyMBill);
            if (rst1 != null)
            {
                lightColor = _repositorySL.FirstOrDefault(l => l.Id == rst1.StorageLocationId).LightColor;
            }
            else if (lightType == 1) // 三色灯
            {
                // 获取当前灯颜色
                var nowLightColor = await _repositorySL.GetAll().Where(r => r.LightState == LightState.On)
                    .GroupBy(l => l.LightColor).Select(g => g.Key).ToListAsync();
                if (!nowLightColor.Contains(LightColor.Green))
                {
                    lightColor = LightColor.Green;
                }
                else if (!nowLightColor.Contains(LightColor.Red))
                {
                    lightColor = LightColor.Red;
                }
                else if (!nowLightColor.Contains(LightColor.Blue))
                {
                    lightColor = LightColor.Blue;
                }
                else
                {
                    // 本次备料勾选混乱
                    throw new LYException("三色灯最多支持三个备料单同时备料,如需备料请先关闭一个备料单");
                }
            }
            else
            {
                // 单色灯支持一个工单备料
                var old = await _repositoryRST.FirstOrDefaultAsync(r => r.IsSend == false);
                if (old != null)
                {
                    throw new LYException("单色灯最多支持一个备料单同时备料,如需备料请先关闭备料单");
                }
            }



            await _repositoryRSHT.DeleteAsync(r => r.ReReadyMBillId == readyM.ReReadyMBill);

            // 记账备料单

            var ReReadyMBill = await _repository.GetAll().Where(r => r.Id == readyM.ReReadyMBill).Include(r => r.WorkBills).ThenInclude(w => w.WorkBill).FirstOrDefaultAsync();

            // 调拨策略
            var ReelMoveMethod = await _repositoryRMM.GetAll().Where(rmm => rmm.Id == ReReadyMBill.ReelMoveMethodId).Include(s => s.OutStorages).FirstOrDefaultAsync();

            // 查询备损数量
            var readyLossQty = await SettingManager.GetSettingValueForTenantAsync<int>("readyLossQty", AbpSession.TenantId.Value);  // readyLossQty

            // 查询强制先进先出天数
            var mustFifoDay = await SettingManager.GetSettingValueForTenantAsync<int>("mustFifoDay", AbpSession.TenantId.Value); // mustFifoDay

            // 添加备料绑定关系
            // 清除老灯
            _repositoryRST.Delete(r => r.ReReadyMBillId == ReReadyMBill.Id);
            _repositoryRSHT.Delete(r => r.ReReadyMBillId == ReReadyMBill.Id);

            foreach (var readyItemDto in readyM.ReadyMBills)
            {
                var readyItem = await _repository.FirstOrDefaultAsync(readyItemDto.Id);
                readyItem.ReadyMStatus = ReadyMStatus.InIssUe;
                readyItem.ReReadyMBillId = ReReadyMBill.Id;
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // 查询当前工单的物料需求详情
            var readyMBs = await _repositoryReadyMBilld.GetAll().Where(r => readyM.ReadyMBills.Select(rm => rm.Id).Contains(r.ReadyMBillId))
                .ToListAsync();

            // 对备料详细进行分组
            var groupReadyMBs = readyMBs.GroupBy(s => s.PartNoId).Select(s => new ReadyMBillDetailed()
            {
                Id = s.FirstOrDefault(sm => sm.ReadyMBillId == ReReadyMBill.Id) != null ?
                     s.FirstOrDefault(sm => sm.ReadyMBillId == ReReadyMBill.Id).Id :
                     s.OrderBy(g => g.Id).FirstOrDefault().Id,
                PartNoId = s.Key,
                DemandQty = s.Sum(rm => rm.Qty) + readyLossQty,
                SendQty = s.Sum(rm => rm.SendQty),
                ReturnQty = s.Sum(rm => rm.ReturnQty)
            }).ToList();


            // 查询沿用工单的余料详情,且剩余数大于0的发料详细
            var followReadyBills = _repository.GetAll().Where(r => r.ReReadyMBillId == readyM.FollowReadyMBill.ReReadyMBillId);

            var surplusReels = _repositoryReadyMBilld.GetAll().Where(re => followReadyBills.Select(r => r.Id).Contains(re.ReadyMBillId))
                .GroupBy(r => r.PartNoId).Select(r => new
                {
                    ReadyMBillDetailedId = r.FirstOrDefault().Id,
                    ReadyMBillDetailed = r.FirstOrDefault(),
                    PartNoId = r.Key,
                    Qty = r.Sum(reel => reel.SendQty) - r.Sum(reel => reel.Qty) - r.Sum(reel => reel.ReturnQty)
                })
                .Where(r => r.Qty > 0);

            #region 按合并后灯需求进行发料
            foreach (var readyMB in groupReadyMBs)
            {
                // 总需求数
                var demandQty = readyMB.DemandQty;

                // 已发数量
                var sendQty = readyMB.SendQty;

                // 计算当前需求数量
                var nowDemandQty = demandQty - sendQty;

                var nowDemandSendQty = nowDemandQty;


                // 先进行工单沿用,工单沿用全部沿用过来
                #region 工单沿用
                if (readyM.FollowReadyMBill != null && readyM.FollowReadyMBill.ReReadyMBillId != null)
                {
                    var surplusReel = await surplusReels.FirstOrDefaultAsync(r => r.PartNoId == readyMB.PartNoId);

                    if (surplusReel != null)
                    {
                        // 有沿用物料,进行物料沿用
                        // 模拟添加发料临时表, 且标记为已发
                        await _repositoryRST.InsertAsync(new ReelSendTemp()
                        {
                            IsActive = true,
                            PartNoId = readyMB.PartNoId,
                            DemandQty = readyMB.DemandQty,
                            IsSend = true,
                            Qty = surplusReel.Qty,
                            SendQty = surplusReel.Qty,
                            ReelMoveMethodId = ReReadyMBill.ReelMoveMethodId,
                            ReReadyMBillId = ReReadyMBill.Id,
                            ReadyMBillDetailedId = readyMB.Id,
                            SlotId = readyMB.SlotId,
                            BOMId = readyMB.BOMId
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();

                        // 已发数量
                        readyMB.SendQty += surplusReel.Qty;
                        // 沿用数量
                        readyMB.FollowQty += surplusReel.Qty;

                        // 更新此单的退料数量
                        surplusReel.ReadyMBillDetailed.ReturnQty += surplusReel.Qty;

                        // 重新计算当前需求数量
                        nowDemandQty -= _repositoryRST.GetAll().Where(rst => rst.PartNoId == readyMB.PartNoId).Sum(rst => rst.Qty);
                        nowDemandSendQty = nowDemandQty;
                        // 添加沿用的发料日志
                        await _reelMoveLog.InsertAsync(new ReelMoveLog()
                        {
                            PartNoId = surplusReel.PartNoId,
                            Qty = surplusReel.Qty,
                            ReadyMBillId = ReReadyMBill.Id,
                            ReadyMBillDetailedId = readyMB.Id,
                            ReelMoveMethodId = ReReadyMBill.ReelMoveMethodId,
                            SlotId = readyMB.SlotId,
                            BOMId = readyMB.BOMId
                        });
                    }
                }
                #endregion

                // 本次挑料

                // 如果需求没有用完 且站位没有发够 接着从库存挑料
                if (nowDemandSendQty > 0)
                {
                    // 查询MPN信息                           
                    var mpn = await _repositoryMPN.FirstOrDefaultAsync(readyMB.PartNoId);

                    // 查询库存中该料号所有物料 在库存中挑料,分为两部分.一部分强制先进先出。一部分按数量挑选
                    var reels = _repositoryReel.GetAll().Where(r =>
                      r.StorageLocationId.Length > 0            // 有库位
                      && r.IsActive     // 有效
                      && r.PartNoId == readyMB.PartNoId   // 料号正确
                      && ReelMoveMethod.OutStorages.Select(s => s.StorageId).Contains(r.StorageId)  // 仓库正确  
                      ).ToList()
                      .Where(r => r.MakeDate.AddDays(mpn.ShelfLife + r.ExtendShelfLife) > DateTime.Now && _repositoryRST.FirstOrDefault(s => s.Id == r.Id) == null) // 未过期                     
                      .ToList(); // 按d/c 进行先进先出排序

                    // && _repositoryRST.FirstOrDefault(s => s.Id == r.Id) == null // 且未被挑料


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
                                reel = reels.Where(r => r.Qty > nowDemandSendQty).OrderBy(r => r.Qty).FirstOrDefault();

                                if (reel == null)
                                {
                                    // 2、如果没有数量大于需求数物料，直接从最大包装开始拿料
                                    reel = reels.OrderByDescending(r => r.Qty).FirstOrDefault();
                                }
                            }
                        }

                        // 模拟添加发料临时表,且标记为已发
                        await _repositoryRST.InsertAsync(new ReelSendTemp()
                        {
                            Id = reel.Id,
                            IsActive = true,
                            PartNoId = readyMB.PartNoId,
                            DemandQty = readyMB.DemandQty,
                            DemandSendQty = nowDemandSendQty,
                            IsSend = false,
                            Qty = reel.Qty,
                            SendQty = reel.Qty,
                            StorageLocationId = reel.StorageLocationId,
                            ReelMoveMethodId = ReReadyMBill.ReelMoveMethodId,
                            ReReadyMBillId = ReReadyMBill.Id,
                            ReadyMBillDetailedId = readyMB.Id,
                            SlotId = readyMB.SlotId,
                            BOMId = readyMB.BOMId
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();


                        // 标记亮灯,真正的亮灯操作。移到专门的亮灯客户端
                        var sl = _repositorySL.Get(reel.StorageLocationId);
                        sl.LightState = LightState.On;
                        sl.LightColor = lightColor;
                        await _repositorySL.UpdateAsync(sl);

                        // 已发数量
                        // readySlotD.SendQty += reel.Qty;
                        reel.SlotId = readyMB.SlotId;

                        // 移除当前料盘,让其不进入下一次挑料
                        reels.Remove(reel);

                        // 重新计算当前需求数
                        nowDemandSendQty -= reel.Qty;
                    }
                }
            }

            #endregion

            // 计算缺料
            foreach (var readyMB in groupReadyMBs)
            {
                var selectQty = _repositoryRST.GetAll().Where(rst => rst.ReadyMBillDetailedId == readyMB.Id).Sum(rst => rst.Qty);
                var pnOneShortQty = readyMB.DemandQty - readyMB.SendQty + readyMB.ReturnQty - selectQty;

                if (pnOneShortQty > 0)
                {
                    //reelShortTemps.Add(new ReelShortTemp()
                    //{
                    //    DemandQty = readyMB.DemandQty,
                    //    DemandSendQty = readyMB.DemandQty - readyMB.SendQty + readyMB.ReturnQty,
                    //    IsActive = true,
                    //    PartNoId = readyMB.PartNoId,
                    //    ReReadyMBillId = ReReadyMBill.Id,
                    //    ShortQty = pnOneShortQty,
                    //    SelectQty = selectQty,
                    //});
                    await _repositoryRSHT.InsertAsync(new ReelShortTemp()
                    {
                        DemandQty = readyMB.DemandQty,
                        DemandSendQty = readyMB.DemandQty - readyMB.SendQty + readyMB.ReturnQty,
                        IsActive = true,
                        PartNoId = readyMB.PartNoId,
                        ReReadyMBillId = ReReadyMBill.Id,
                        ShortQty = pnOneShortQty,
                        SelectQty = selectQty,
                    });
                    await CurrentUnitOfWork.SaveChangesAsync();

                }

                // 更新每条数据的最终挑料数量
                foreach (var item in _repositoryRST.GetAll().Where(r => r.ReadyMBillDetailedId == readyMB.Id))
                {
                    item.SelectQty = selectQty;
                }
            }


            //foreach (var item in reelSendTempDtos)
            //{
            //    await _repositoryRST.InsertAsync(item);
            //}

            //foreach (var item in reelShortTemps)
            //{
            //    await _repositoryRSHT.InsertAsync(item);
            //}


            readyMResultDto.Success = true;
            readyMResultDto.Msg = "备料成功,打开备料看版查询详情.";

            await CurrentUnitOfWork.SaveChangesAsync();

            // 亮灯
            var lights = await _repositorySL.GetAllListAsync(s => s.LightState != LightState.Off && s.LightColor == lightColor);

            //小灯
            var simlights = lights.Select(l => new StorageLight() { ContinuedTime = 10, LightColor = lightColor, LightOrder = 1, MainBoardId = l.MainBoardId, RackPositionId = l.PositionId }).ToList();

            LightService.LightOrder(simlights);

            var houselights = simlights.Select(l => new HouseLight() { HouseLightSide = 1, LightColor = lightColor, LightOrder = 1, MainBoardId = l.MainBoardId }).Distinct().ToList();

            LightService.HouseOrder(houselights);

            return readyMResultDto;
        }

        public async Task<ReadyMResultDto> ReadyFirstM(ReadyMDto readyM)
        {
            if (readyM.ReadyMBills.GroupBy(s => new { s.Linestr }).Count() > 1)
            {
                throw new LYException("合并工单必须为相同线别");
            }

            // 判断本次备料单是否全部有备料记录
            var rms = await _repository.GetAll().Where(r => readyM.ReadyMBills.Select(rm => rm.Id).Contains(r.Id) && r.ReReadyMBillId != null).Select(r => r.ReReadyMBillId).Distinct().ToListAsync();

            // 有过备料记录的
            if (rms.Count > 1)
            {
                // 本次备料勾选混乱
                throw new LYException("若本次备料包含多个备料单,那么本次备料必须和历史备料勾选一致");
            }
            else if (rms.Count == 1)
            {
                if (rms[0] != readyM.ReReadyMBill)
                {
                    throw new LYException("本次备料记账备料单[" + readyM.ReReadyMBill + "]和历史记账备料单[" + rms[0] + "]不一致");
                }
            }
            // 获取灯配置,是单灯还是多灯
            // var settinglightType = await _repositoryT.FirstOrDefaultAsync(c => c.TenantId == AbpSession.TenantId && c.Name == "lightIsRGB");
            var lightType = SettingManager.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.TenantId.Value);
            var lightColor = LightColor.Default;

            var rst1 = _repositoryRST.FirstOrDefault(r => r.ReReadyMBillId == readyM.ReReadyMBill);
            if (rst1 != null)
            {
                lightColor = _repositorySL.FirstOrDefault(l => l.Id == rst1.StorageLocationId).LightColor;
            }
            else if (lightType == 1) // 三色灯
            {
                // 获取当前灯颜色
                // 查询是否有备料记录


                var nowLightColor = await _repositorySL.GetAll().Where(r => r.LightState == LightState.On)
                    .GroupBy(l => l.LightColor).Select(g => g.Key).ToListAsync();
                if (!nowLightColor.Contains(LightColor.Green))
                {
                    lightColor = LightColor.Green;
                }
                else if (!nowLightColor.Contains(LightColor.Red))
                {
                    lightColor = LightColor.Red;
                }
                else if (!nowLightColor.Contains(LightColor.Blue))
                {
                    lightColor = LightColor.Blue;
                }
                else
                {
                    // 本次备料勾选混乱
                    throw new LYException("三色灯最多支持三个备料单同时备料,如需备料请先关闭一个备料单");
                }
            }
            else
            {
                // 单色灯支持一个工单备料
                var old = await _repositoryRST.FirstOrDefaultAsync(r => r.IsSend == false);
                if (old != null)
                {
                    throw new LYException("单色灯最多支持一个备料单同时备料,如需备料请先关闭备料单");
                }
            }

            // 备首套料
            ReadyMResultDto readyMResultDto = new ReadyMResultDto() { ReReadyMBillId = readyM.ReReadyMBill };
            //List<ReelSendTemp> reelSendTempDtos = new List<ReelSendTemp>();
            //List<ReelShortTemp> reelShortTemps = new List<ReelShortTemp>();

            // 记账备料单
            var ReReadyMBill = await _repository.GetAll().Where(r => r.Id == readyM.ReReadyMBill).Include(r => r.WorkBills).ThenInclude(w => w.WorkBill).FirstOrDefaultAsync();

            // 添加备料绑定关系

            foreach (var readyItemDto in readyM.ReadyMBills)
            {
                var readyItem = await _repository.FirstOrDefaultAsync(readyItemDto.Id);
                readyItem.ReadyMStatus = ReadyMStatus.InIssUe;
                readyItem.ReReadyMBillId = ReReadyMBill.Id;
            }

            // 获取首套料工单
            var firstWoBill = ReReadyMBill.WorkBills.FirstOrDefault().WorkBill;
            await _repositoryRSHT.DeleteAsync(r => r.ReReadyMBillId == readyM.ReReadyMBill);

            // 调拨策略
            var ReelMoveMethod = await _repositoryRMM.GetAll().Where(rmm => rmm.Id == ReReadyMBill.ReelMoveMethodId).Include(s => s.OutStorages).FirstOrDefaultAsync();

            // 查询最低备料数量
            var readyFirstMinimumQty = SettingManager.GetSettingValueForTenant<int>("readyFirstMinimumQty", AbpSession.TenantId.Value); // int.Parse((await _repositoryT.FirstOrDefaultAsync(c => c.TenantId == AbpSession.TenantId && c.Name == "readyFirstMinimumQty")).Value);

            // 查询当前工单的物料需求详情
            var readyMBs = await _repositoryReadyMBilld.GetAll().Where(r => readyM.ReadyMBills.Select(rm => rm.Id).Contains(r.ReadyMBillId))
                .ToListAsync();

            var groupReadyMBs = readyMBs.GroupBy(s => s.PartNoId).Select(s => new ReadyMBillDetailedDto()
            {
                Id = s.FirstOrDefault().Id,
                PartNoId = s.Key,
                DemandQty = s.Sum(rm => rm.Qty),
                SendQty = s.Sum(rm => rm.SendQty),
                ReturnQty = s.Sum(rm => rm.ReturnQty)
            }).ToList();

            // 查询料站表
            var slots = await _repositorySlot.GetAll().Where(s => s.ProductId == firstWoBill.ProductId && s.LineId == firstWoBill.LineId && s.IsActive)
                .OrderBy(r => (int)r.BoardSide * -1)
                .ThenBy(r => r.Index)
                .ToListAsync();

            // 记账小车库位料站表关联
            Dictionary<string, StorageLocation> dicShelfCarSlotMap = new Dictionary<string, StorageLocation>();

            // 查询小车库位
            var shelfCars = await _repositorySL.GetAll().Where(s => s.Code == readyM.ShelfCar).ToListAsync();

            if (shelfCars.Count == 0)
            {
                throw new LYException(string.Format("输入小车{0}不存在", readyM.ShelfCar));
            }

            // 小车全部灭灯
            foreach (var car in shelfCars)
            {
                car.LightState = LightState.Off;
            }

            // 第一个table
            string lastTable = slots[0].Machine + slots[0].Table;
            int nowPositionId = 1;
            // 小车进行料站表绑定并且空位保存
            foreach (var slot in slots)
            {
                string nowTable = slot.Machine + slot.Table;
                var shelfCar = shelfCars.FirstOrDefault(r => r.PositionId == nowPositionId);
                if (lastTable == nowTable)
                {
                    dicShelfCarSlotMap.Add(slot.Id, shelfCar);
                }
                else
                {
                    // 标记为亮灯
                    shelfCar.LightState = LightState.On;
                    shelfCar.LightColor = lightColor;
                    // 多用一个位置
                    nowPositionId++;
                    lastTable = nowTable;
                    shelfCar = shelfCars.FirstOrDefault(r => r.PositionId == nowPositionId);
                    dicShelfCarSlotMap.Add(slot.Id, shelfCar);
                }
                nowPositionId++;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            // 按料站表备首套料
            foreach (var slot in slots)
            {

                // 获取MPQ数量
                var mpn = await _repositoryMPN.FirstOrDefaultAsync(slot.PartNoId);

                // 查询发料详细中对应行
                var readyBM = groupReadyMBs.FirstOrDefault(r => r.PartNoId == slot.PartNoId);

                // 查询库存中该料号符合条件物料并且按照 d/c 排序
                var reels = await _repositoryReel.GetAll().Where(r =>
                   r.StorageLocationId.Length > 0            // 有库位
                   && r.IsActive     // 有效
                   && r.PartNoId == slot.PartNoId  // 料号正确
                   && (r.Qty >= readyFirstMinimumQty)
                   && ReelMoveMethod.OutStorages.Select(s => s.StorageId).Contains(r.StorageId)
                   && _repositoryRST.FirstOrDefault(s => s.Id == r.Id) == null
                 ) // 没有过期物料 d/c 加保质期  加 延续时间 减 提前预警时间 大于 当前时间为合格
                  .ToListAsync();

                reels = reels.Where(r => _repositoryRST.FirstOrDefault(s => s.Id == r.Id) == null).ToList();

                reels = reels.Where(r => r.MakeDate.AddDays(mpn.ShelfLife + r.ExtendShelfLife) > DateTime.Now)
                  .OrderBy(r => r.MakeDate).ToList(); // 按d/c 进行先进先出排序

                var reel = reels.FirstOrDefault();

                if (reel != null)  // 有找到物料,直接备出第一盘料,且跳出当前循环
                {
                    // 添加发料信息
                    //reelSendTempDtos.Add();

                    await _repositoryRST.InsertAsync(new ReelSendTemp()
                    {
                        Id = reel.Id,
                        IsActive = true,
                        PartNoId = reel.PartNoId,
                        DemandQty = readyBM.DemandQty,
                        DemandSendQty = reel.Qty,
                        IsSend = false,
                        Qty = reel.Qty,
                        SendQty = reel.Qty,
                        StorageLocationId = reel.StorageLocationId,
                        ReelMoveMethodId = ReReadyMBill.ReelMoveMethodId,
                        ReReadyMBillId = ReReadyMBill.Id,
                        ReadyMBillDetailedId = readyBM.Id,
                        SlotId = slot.Id,
                        FisrtStorageLocationId = dicShelfCarSlotMap[slot.Id].Id
                    });
                    await CurrentUnitOfWork.SaveChangesAsync();

                    reel.SlotId = slot.Id;

                    // 标记亮灯,真正的亮灯操作。移到专门的亮灯客户端
                    var sl = _repositorySL.Get(reel.StorageLocationId);
                    sl.LightState = LightState.On;
                    sl.LightColor = lightColor;
                    break;
                }

                // 查询是否找到首料
                var firstReel = _repositoryRST.FirstOrDefault(r => r.SlotId == slot.Id);
                if (firstReel == null)
                {
                    // 如果没有找到首料,添加该站位的首料缺料信息
                    await _repositoryRSHT.InsertAsync(new ReelShortTemp()
                    {
                        DemandQty = readyBM.DemandQty,  // 用所有料的数量总和代替
                        DemandSendQty = readyFirstMinimumQty,  // 需发为最小数量
                        IsActive = true,
                        PartNoId = readyBM.PartNoId, // 为所有可发物料
                        ReReadyMBillId = ReReadyMBill.Id,
                        ShortQty = readyFirstMinimumQty,
                        SlotId = slot.Id,
                        SelectQty = 0
                    });
                }
            }

            readyMResultDto.Success = true;
            readyMResultDto.Msg = "备料成功,打开备料看吧查询详情.";

            await CurrentUnitOfWork.SaveChangesAsync();

            // 亮灯
            var lights = await _repositorySL.GetAllListAsync(s => s.LightState != LightState.Off && s.LightColor == lightColor);

            //小灯
            var simlights = lights.Select(l => new
            {
                l.MainBoardId,
                RackPositionId = l.PositionId
            }).Select(s => new StorageLight()
            {
                ContinuedTime = 10,
                LightOrder = 1,
                MainBoardId = s.MainBoardId,
                LightColor = lightColor,
                RackPositionId = s.RackPositionId
            }).ToList();

            LightService.LightOrder(simlights);

            // 灯塔
            var houselights = simlights.Where(r => r.MainBoardId.ToString().Length != 3).Select(l => new
            {
                l.MainBoardId
            }).Distinct()
            .Select(s => new HouseLight()
            {
                MainBoardId = s.MainBoardId,
                HouseLightSide = 1,
                LightOrder = 1,
                LightColor = lightColor
            })
            .ToList();
            LightService.HouseOrder(houselights);

            return readyMResultDto;
        }

        public async Task CancelReadyM(string readyMid)
        {
            // 查询当前备料单临时表
            var sendTemp = await _repositoryRST.GetAll().Where(r => r.ReReadyMBillId == readyMid).Select(r => r.StorageLocationId).ToListAsync();

            // 查询当前备料单库位
            var lights = await _repositorySL.GetAll().Where(l => sendTemp.Contains(l.Id)
            && l.LightState == LightState.On
            ).ToListAsync();

            // 库位灭灯
            //小灯
            var simlights = lights.Select(l => new
            {
                l.MainBoardId,
                RackPositionId = l.PositionId,
                l.LightColor
            }).Select(s => new StorageLight()
            {
                ContinuedTime = 10,
                LightOrder = 0,
                MainBoardId = s.MainBoardId,
                LightColor = s.LightColor,
                RackPositionId = s.RackPositionId
            }).ToList();

            var allLightOrder = simlights.GroupBy(s => new { s.MainBoardId, s.LightOrder, s.LightColor }).Select(s => new AllLight()
            {
                LightColor = s.Key.LightColor,
                LightOrder = s.Key.LightOrder,
                MainBoardId = s.Key.MainBoardId
            }).ToList();

            // 单颜色全灭
            LightService.AllLightOrder(allLightOrder);

            // 灯塔
            var houselights = simlights.Where(r => r.MainBoardId.ToString().Length != 3).Select(l => new
            {
                l.MainBoardId,
                l.LightColor
            }).Distinct()
            .Select(s => new HouseLight()
            {
                MainBoardId = s.MainBoardId,
                HouseLightSide = 1,
                LightOrder = 0,
                LightColor = s.LightColor
            })
            .ToList();
            LightService.HouseOrder(houselights);

            if (_reelMoveLog.FirstOrDefault(r => r.ReceivedReelBillId == readyMid) == null)
            {
                // 记账备料单
                var ReReadyMBill = await _repository.GetAll().Where(r => r.ReReadyMBillId == readyMid).ToListAsync();

                // 添加备料绑定关系

                foreach (var readyItemDto in ReReadyMBill)
                {
                    var readyItem = await _repository.FirstOrDefaultAsync(readyItemDto.Id);
                    readyItem.ReadyMStatus = ReadyMStatus.Ready;
                }
            }

            // 更新库位表
            foreach (var item in lights)
            {
                item.LightState = LightState.Off;
            }

            // 删除临时表
            await _repositoryRST.DeleteAsync(r => r.ReReadyMBillId == readyMid);
            await _repositoryRSHT.DeleteAsync(r => r.ReReadyMBillId == readyMid);

        }

        public async Task CloseReadyM(string readyMid)
        {
            // 查询当前备料单临时表
            var sendTemp = await _repositoryRST.GetAll().Where(r => r.ReReadyMBillId == readyMid).Select(r => r.StorageLocationId).ToListAsync();

            // 查询当前备料单库位
            var lights = await _repositorySL.GetAll().Where(l => sendTemp.Contains(l.Id)
            && l.LightState == LightState.On
            ).ToListAsync();

            // 库位灭灯
            //小灯
            var simlights = lights.Select(l => new
            {
                l.MainBoardId,
                RackPositionId = l.PositionId,
                l.LightColor
            }).Select(s => new StorageLight()
            {
                ContinuedTime = 10,
                LightOrder = 0,
                MainBoardId = s.MainBoardId,
                LightColor = s.LightColor,
                RackPositionId = s.RackPositionId
            }).ToList();

            // LightService.LightOrder(simlights);

            var allLightOrder = simlights.GroupBy(s => new { s.MainBoardId, s.LightOrder, s.LightColor }).Select(s => new AllLight()
            {
                LightColor = s.Key.LightColor,
                LightOrder = s.Key.LightOrder,
                MainBoardId = s.Key.MainBoardId
            }).ToList();

            // 单颜色全灭
            LightService.AllLightOrder(allLightOrder);

            // 灯塔
            var houselights = simlights.Where(r => r.MainBoardId.ToString().Length != 3).Select(l => new
            {
                l.MainBoardId,
                l.LightColor
            }).Distinct()
            .Select(s => new HouseLight()
            {
                MainBoardId = s.MainBoardId,
                HouseLightSide = 1,
                LightOrder = 0,
                LightColor = s.LightColor
            })
            .ToList();
            LightService.HouseOrder(houselights);


            // 记账备料单
            var ReReadyMBill = await _repository.GetAll().Where(r => r.ReReadyMBillId == readyMid).ToListAsync();

            // 添加备料绑定关系

            foreach (var readyItemDto in ReReadyMBill)
            {
                var readyItem = await _repository.FirstOrDefaultAsync(readyItemDto.Id);
                readyItem.ReadyMStatus = ReadyMStatus.Finish;
            }

            // 更新库位表
            foreach (var item in lights)
            {
                item.LightState = LightState.Off;
            }

            // 删除临时表
            await _repositoryRST.DeleteAsync(r => r.ReReadyMBillId == readyMid);
            await _repositoryRSHT.DeleteAsync(r => r.ReReadyMBillId == readyMid);
        }
    }
}
