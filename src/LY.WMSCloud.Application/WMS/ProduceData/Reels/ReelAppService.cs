using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Linq.Extensions;
using AutoMapper;
using LY.WMSCloud.Base;
using LY.WMSCloud.CommonService;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.ProduceData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.BaseData.BarCodeAnalysiss;
using LY.WMSCloud.WMS.ProduceData.ReceivedReelBills.Dto;
using LY.WMSCloud.WMS.ProduceData.Reels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.ProduceData.Reels
{
    public class ReelAppService : ServiceBase<Reel, ReelDto, string>, IReelAppService
    {
        readonly IWMSRepositories<Reel, string> _repository;
        readonly IWMSRepositories<ReelMoveLog, string> _repositoryReelMoveLog;
        readonly IWMSRepositories<StorageLocation, string> _repositorysl;
        readonly IWMSRepositories<Storage, string> _repositorysStorage;
        readonly IWMSRepositories<StorageArea, string> _repositorysStorageA;
        readonly IWMSRepositories<MPNStorageAreaMap, string> _repositorysMPNA;
        readonly IWMSRepositories<MPN, string> _repositorympn;
        readonly IWMSRepositories<ReelMoveMethod, string> _repositoryRMM;
        readonly IWMSRepositories<Slot, string> _repositorySlot;
        readonly IBarCodeAnalysisAppService _barCodeAnalysisAppService;
        readonly IWMSRepositories<ReadyMBillDetailed, string> _repositoryReadyMBilld;
        readonly IWMSRepositories<ReadyMBill, string> _repositoryReadyMBill;
        readonly IWMSRepositories<ReelSendTemp, string> _repositoryRST;
        readonly IWMSRepositories<ReelSupplyTemp, string> _repositoryReelSupplyTemp;
        // readonly IWMSRepositories<ReadySlot, string> _repositoryReadySlot;

        readonly LightService LightService;
        readonly IWMSRepositories<ReceivedReelBill, string> _repositoryrrb;
        public ReelAppService(
            IWMSRepositories<Reel, string> repository,
            IWMSRepositories<MPN, string> repositorympn,
            IBarCodeAnalysisAppService barCodeAnalysisAppService,
            IWMSRepositories<StorageLocation, string> repositorysl,
            IWMSRepositories<ReelSupplyTemp, string> repositoryReelSupplyTemp,
            IWMSRepositories<ReelMoveLog, string> repositoryReelMoveLog,
            IWMSRepositories<ReadyMBillDetailed, string> repositoryReadyMBilld,
            IWMSRepositories<ReceivedReelBill, string> repositoryrrb,
            // IWMSRepositories<ReadySlot, string> repositoryReadySlot,
            IWMSRepositories<ReadyMBill, string> repositoryReadyMBill,
            IWMSRepositories<StorageArea, string> repositorysStorageA,
            IWMSRepositories<MPNStorageAreaMap, string> repositorysMPNA,
            IWMSRepositories<Storage, string> repositorysStorage,
            IWMSRepositories<Slot, string> repositorySlot,
            LightService lightService,
            IWMSRepositories<ReelSendTemp, string> repositoryRST,
            IWMSRepositories<ReelMoveMethod, string> repositoryRMM) : base(repository)
        {
            _repository = repository;
            _barCodeAnalysisAppService = barCodeAnalysisAppService;
            _repositoryRMM = repositoryRMM;
            _repositorympn = repositorympn;
            _repositorysl = repositorysl;
            _repositoryReelMoveLog = repositoryReelMoveLog;
            _repositoryReadyMBilld = repositoryReadyMBilld;
            _repositoryRST = repositoryRST;
            _repositoryReelSupplyTemp = repositoryReelSupplyTemp;
            _repositoryrrb = repositoryrrb;
            _repositorysStorageA = repositorysStorageA;
            // _repositoryReadySlot = repositoryReadySlot;
            LightService = lightService;
            _repositoryReadyMBill = repositoryReadyMBill;
            _repositorySlot = repositorySlot;
            _repositorysMPNA = repositorysMPNA;
            _repositorysStorage = repositorysStorage;
        }

        public async Task BrightByPartNoIds(LightOrderDto[] input)
        {
            // var settinglightType = SettingManager.GetSettingValueForTenant("lightIsRGB", AbpSession.TenantId.Value);// await _repositoryT.FirstOrDefaultAsync(c => c.TenantId == AbpSession.TenantId && c.Name == "lightIsRGB");
            var lightType = SettingManager.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.TenantId.Value);
            var lightColor = LightColor.Default;
            if (lightType == 1)
            {
                lightColor = LightColor.Green;
            }

            // 查询料号库存
            List<string> reelIds = new List<string>();
            foreach (var item in input)
            {
                var reelsOne = _repository.GetAll().Where(r => r.PartNoId == item.ReelOrPns && r.StorageId == item.StorageId).Select(r => r.Id).ToList();
                reelIds.AddRange(reelsOne);
            }

            var lights = await _repositorysl.GetAll().Where(r => reelIds.Contains(r.ReelId)).GroupBy(r => new StorageLight
            {
                RackPositionId = r.PositionId,
                ContinuedTime = 10,
                LightColor = lightColor,
                LightOrder = 1,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToListAsync();

            // 小灯
            LightService.LightOrder(lights);


            // 灯塔
            var mains = lights.GroupBy(r => new AllLight
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToList();

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 1,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 0
            }).ToList());

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 1,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 1
            }).ToList());
        }

        public async Task BrightByReelIds(LightOrderDto[] input)
        {
            var lightType = SettingManager.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.TenantId.Value);
            var lightColor = LightColor.Default;
            if (lightType == 1)
            {
                lightColor = LightColor.Green;
            }
            List<string> reelIds = new List<string>();
            foreach (var item in input)
            {
                var reelsOne = _repository.GetAll().Where(r => r.Id == item.ReelOrPns && r.StorageId == item.StorageId).Select(r => r.Id).ToList();
                reelIds.AddRange(reelsOne);
            }

            // 查询料号库存
            // var reels = _repository.GetAll().Where(r => input.ReelOrPns.Contains(r.Id) && r.StorageId == input.StorageId).Select(r => r.Id).ToList();

            var lights = await _repositorysl.GetAll().Where(r => reelIds.Contains(r.ReelId)).GroupBy(r => new StorageLight
            {
                RackPositionId = r.PositionId,
                ContinuedTime = 10,
                LightOrder = 1,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToListAsync();

            // 小灯
            LightService.LightOrder(lights);

            // 灯塔
            var mains = lights.GroupBy(r => new AllLight
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToList();

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightColor = lightColor,
                LightOrder = 1,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 0
            }).ToList());

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 1,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 1
            }).ToList());
        }

        public async Task ExtinguishedByPartNoIds(LightOrderDto[] input)
        {
            var lightType = SettingManager.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.TenantId.Value);
            var lightColor = LightColor.Default;
            if (lightType == 1)
            {
                lightColor = LightColor.Green;
            }
            // 查询料号库存
            List<string> reelIds = new List<string>();
            foreach (var item in input)
            {
                var reelsOne = _repository.GetAll().Where(r => r.PartNoId == item.ReelOrPns && r.StorageId == item.StorageId).Select(r => r.Id).ToList();
                reelIds.AddRange(reelsOne);
            }

            var lights = await _repositorysl.GetAll().Where(r => reelIds.Contains(r.ReelId)).GroupBy(r => new StorageLight
            {
                RackPositionId = r.PositionId,
                ContinuedTime = 10,
                LightColor = lightColor,
                LightOrder = 0,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToListAsync();

            // 小灯
            LightService.LightOrder(lights);


            // 灯塔
            var mains = lights.GroupBy(r => new AllLight
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToList();

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 0
            }).ToList());

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 1
            }).ToList());
        }

        public async Task ExtinguishedByReelIds(LightOrderDto[] input)
        {
            var lightType = SettingManager.GetSettingValueForTenant<int>("lightIsRGB", AbpSession.TenantId.Value);
            var lightColor = LightColor.Default;
            if (lightType == 1)
            {
                lightColor = LightColor.Green;
            }
            // 查询料号库存
            List<string> reelIds = new List<string>();
            foreach (var item in input)
            {
                var reelsOne = _repository.GetAll().Where(r => r.Id == item.ReelOrPns && r.StorageId == item.StorageId).Select(r => r.Id).ToList();
                reelIds.AddRange(reelsOne);
            }

            var lights = await _repositorysl.GetAll().Where(r => reelIds.Contains(r.ReelId)).GroupBy(r => new StorageLight
            {
                RackPositionId = r.PositionId,
                ContinuedTime = 10,
                LightColor = lightColor,
                LightOrder = 0,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToListAsync();

            // 小灯
            LightService.LightOrder(lights);


            // 灯塔
            var mains = lights.GroupBy(r => new AllLight
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId
            }).Select(r => r.Key).ToList();

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 0
            }).ToList());

            LightService.HouseOrder(lights.Select(r => new HouseLight()
            {
                LightOrder = 0,
                LightColor = lightColor,
                MainBoardId = r.MainBoardId,
                HouseLightSide = 1
            }).ToList());
        }

        [HttpPost]
        public async Task<PagedResultDto<GroupReelDto>> GetGroupReel(PagedResultRequestInput input)
        {
            // 先按参数查询数据
            var query = _repository.DynamicQuery(input);

            var res = query.GroupBy(r => new { r.PartNoId, r.StorageId }).Select(r => new GroupReelDto()
            {
                PartNoId = r.Key.PartNoId,
                StorageId = r.Key.StorageId,
                ReelCount = r.Count(),
                TotalQty = r.Sum(reel => reel.Qty)
            });

            var tasksCount = await res.CountAsync();

            var taskList = res.PageBy(input).ToList();

            return new PagedResultDto<GroupReelDto>(tasksCount, taskList);
        }

        [HttpPost]
        public async Task<PagedResultDto<ReelOutLifeDto>> GetOutLifeReel(PagedResultRequestInput input)
        {
            // 查询仓库超期物料
            // var overdueDaySet = SettingManager.GetSettingValueForTenant<double>("overdueDay", AbpSession.TenantId.Value);// (await _repositoryT.FirstOrDefaultAsync(c => c.TenantId == AbpSession.TenantId && c.Name == "overdueDay")).Value;

            double overdueDay = SettingManager.GetSettingValueForTenant<double>("overdueDay", AbpSession.TenantId.Value);

            //Logger.Info(overdueDay.ToString());   

            DateTime dt = DateTime.Now.AddDays(overdueDay);

            var outLifeReelsIQ = _repository.GetAllIncluding(r => r.PartNo).Where(r => r.MakeDate <= dt.AddDays((r.ExtendShelfLife + r.PartNo.ShelfLife) * -1) && r.StorageLocationId.Length > 0);

            //var sql = outLifeReelsIQ.ToSql();
            //Logger.Info(outLifeReelsIQ.ToSql());

            // (double)(r.PartNo.ShelfLife + r.ExtendShelfLife)  .AddDays((r.PartNo.ShelfLife + r.ExtendShelfLife) * -1)

            var outLifeReels = await outLifeReelsIQ.ToListAsync();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Reel, ReelOutLifeDto>()
                    .ForMember(x => x.ShelfLife, opt => opt.MapFrom(x => x.PartNo.ShelfLife))
                    .ForMember(x => x.WarningDay, opt => opt.MapFrom(x => overdueDay))
                    .ForMember(x => x.OutLifeDate, opt => opt.MapFrom(x => x.MakeDate.AddDays(x.ExtendShelfLife + x.PartNo.ShelfLife)))
                    .ForMember(x => x.OutLifeDay, opt => opt.MapFrom(x => (DateTime.Now.Date - x.MakeDate.AddDays(x.ExtendShelfLife + x.PartNo.ShelfLife)).Days))
                    .ForMember(x => x.WarnLifeDate, opt => opt.MapFrom(x => x.MakeDate.AddDays(x.ExtendShelfLife + x.PartNo.ShelfLife - overdueDay)))
                    .ForMember(x => x.WarnLifeDay, opt => opt.MapFrom(x => (DateTime.Now.Date - x.MakeDate.AddDays(x.ExtendShelfLife + x.PartNo.ShelfLife - overdueDay)).Days))
                    .ForMember(x => x.OutLifeType, opt => opt.MapFrom(x => (x.MakeDate > DateTime.Now.AddDays((x.ExtendShelfLife + x.PartNo.ShelfLife) * -1)) ? OutLifeType.Warning : OutLifeType.OutLife))
                    ;
            }
               );


            var query = config.CreateMapper().Map<List<ReelOutLifeDto>>(outLifeReels).AsQueryable().DynamicQuery(input);

            var tasksCount = query.Count();

            //默认的分页方式
            //var taskList = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            //ABP提供了扩展方法PageBy分页方式
            var taskList = query.PageBy(input).ToList();

            return new PagedResultDto<ReelOutLifeDto>(tasksCount, taskList);
        }


        public async Task<ReelMoveResDto> ReelMove(ReelMoveDto inputDto)
        {
            // Logger.Info("tm1" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss ffff"));

            // 获取调拨策略信息
            ReelMoveMethod reelMoveMethod = await _repositoryRMM.GetAll().Where(m => m.Id == inputDto.ReelMoveMethodId).Include(m => m.OutStorages).FirstOrDefaultAsync();

            // 条码解析信息
            ReelDto reelDto = null;

            // 料盘信息
            Reel reel = null;

            // 料盘物料信息
            MPN reelMpn = null;

            // 日志信息
            ReelMoveLog reelMoveLog = new ReelMoveLog() { ReelMoveMethodId = reelMoveMethod.Id };

            // 返回信息
            ReelMoveResDto resDto = new ReelMoveResDto() { IsContinuity = inputDto.IsContinuity, Msg = "操作成功", NextShlefLab = inputDto.ShlefLab };

            // 下架货架信息
            StorageLocation shelfUp = null;

            // 上架货架信息
            StorageLocation shelfOn = null;

            // 条码解析
            Func<Task> BarCodeAnalysis = new Func<Task>(async () =>
            {
                if (reelDto == null)
                {
                    var analysisRes = await _barCodeAnalysisAppService.Analysis(new BaseData.BarCodeAnalysiss.Dto.AnalysisDto() { BarCode = inputDto.BarCode, DtoName = "ReelDto" });
                    if (!analysisRes.Success)
                    {
                        resDto.Msg = analysisRes.Msg;
                        throw new LYException(resDto.Msg);
                    }
                    else
                    {
                        reelDto = analysisRes.Result as ReelDto;
                    }
                }
            });

            // 检查料盘
            Func<Task> CheckReel = new Func<Task>(async () =>
            {
                if (reelDto == null)
                {
                    await BarCodeAnalysis();
                }

                if (reel == null)
                {
                    reel = await _repository.FirstOrDefaultAsync(reelDto.Id);
                    if (reel == null)
                    {
                        throw new LYException(reelDto.Id + "不存在,请先进行料卷注册");
                    }

                    reelMoveLog.ReelId = reel.Id;
                    reelMoveLog.PartNoId = reel.PartNoId;
                    reelMoveLog.Qty = reel.Qty;
                }

            });

            // 检查库位信息
            Func<Task> CheckShelfOn = new Func<Task>(async () =>
            {
                if (shelfOn == null)
                {
                    shelfOn = await _repositorysl.FirstOrDefaultAsync(inputDto.ShlefLab);

                    if (shelfOn == null)
                    {
                        resDto.Msg = "库位[" + inputDto.ShlefLab + "]不存在";
                        throw new LYException(resDto.Msg);
                    }

                    reelMoveLog.StorageLocationId = shelfOn.Id;
                }

            });

            // 获取物料信息
            Func<Task> CheckMPN = new Func<Task>(async () =>
            {
                if (reelMpn == null)
                {
                    reelMpn = await _repositorympn.FirstOrDefaultAsync(reelDto.PartNoId);
                    if (reelMpn == null)
                    {
                        resDto.Msg = "料号[" + reel.PartNoId + "]未维护";
                        throw new LYException(resDto.Msg);
                    }
                }

            });

            // 获取物料库位信息
            Func<Task> GetShelfUp = new Func<Task>(async () =>
            {
                shelfUp = await _repositorysl.FirstOrDefaultAsync(reel.StorageLocationId);
                if (shelfUp == null)
                {
                    resDto.Msg = "料盘[" + reel.Id + "]未上架";
                    throw new LYException(resDto.Msg);
                }
                reelMoveLog.StorageLocationId = shelfUp.Id;
            });

            foreach (var allocationType in reelMoveMethod.AllocationTypes)
            {

                switch (allocationType)
                {
                    case AllocationType.Move:
                    case AllocationType.OnSL:
                    case AllocationType.UpSl:
                    case AllocationType.Send:
                    case AllocationType.Return:
                    case AllocationType.Received:
                    case AllocationType.SendFirstReel:
                    case AllocationType.SupplyReel:
                        if (reel == null)     // 进行料卷检查
                        {
                            await CheckReel();
                        }

                        switch (allocationType)
                        {
                            case AllocationType.Move: // 转仓

                                #region  转仓                             
                                // 检查料卷调出仓是否合法
                                if (!reelMoveMethod.OutStorages.Select(s => s.StorageId).Contains(reel.StorageId))
                                {

                                    resDto.Msg = "料卷不属于该调拨策略的调出仓[" + string.Join('|', reelMoveMethod.OutStorages.Select(s => s.StorageId)) + "]";
                                    throw new LYException(resDto.Msg);
                                }
                                // 进行转仓
                                reel.StorageId = reelMoveMethod.InStorageId;

                                #endregion
                                break;
                            case AllocationType.OnSL: // 上架

                                #region 上架
                                if (shelfOn == null)
                                {
                                    await CheckShelfOn(); // 检查库位信息
                                }

                                // 库位是否有料
                                if (shelfOn.ReelId != null)
                                {
                                    resDto.Msg = "库位[" + inputDto.ShlefLab + "]已绑定料卷[" + shelfOn.ReelId + "]";
                                    throw new LYException(1, resDto.Msg);
                                }

                                // 库位是否在转入仓
                                if (shelfOn.StorageId != reelMoveMethod.InStorageId)
                                {
                                    resDto.Msg = "库位[" + inputDto.ShlefLab + "]不不属于[" + reelMoveMethod.InStorageId + "]仓";
                                    throw new LYException(1, resDto.Msg);
                                }

                                // 料卷是否上架
                                if (reel.StorageLocationId != null)
                                {
                                    resDto.Msg = "料卷[" + reel.Id + "]已经上架[" + reel.StorageLocationId + "]";
                                    throw new LYException(resDto.Msg);
                                }

                                // 查询物料可用区域

                                var mpnA = _repositorysMPNA.GetAll().Where(r => r.MPNId == reel.PartNoId).ToArray();

                                if (mpnA.Length > 0 && !mpnA.Select(r => r.StorageAreaId).Contains(shelfOn.StorageAreaId))
                                {
                                    resDto.Msg = "物料[" + reel.PartNoId + "]允许上架分区为[" + string.Join('|', mpnA.Select(r => r.StorageAreaId)) + "]";
                                    throw new LYException(resDto.Msg);
                                }
                                else
                                {

                                    if (shelfOn.StorageAreaId != null && !mpnA.Select(r => r.StorageAreaId).Contains(shelfOn.StorageAreaId))
                                    {
                                        resDto.Msg = "分区[" + shelfOn.StorageAreaId + "]不允许物料[" + reel.PartNoId + "]上架";
                                        throw new LYException(resDto.Msg);
                                    }
                                }
                                // Logger.Info("tm91" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss ffff"));
                                // 寻找下一个空库位
                                var nextShelf = await _repositorysl.GetAll()
                                    .Where(s =>
                                        s.MainBoardId == shelfOn.MainBoardId &&
                                        s.PositionId > shelfOn.PositionId &&
                                        (s.ReelId == null || s.ReelId.Length == 0))
                                    .OrderBy(s => s.PositionId)
                                    .FirstOrDefaultAsync();
                                // Logger.Info("tm92" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss ffff"));
                                if (nextShelf != null)
                                {
                                    resDto.NextShlefLab = nextShelf.Id;
                                }
                                else
                                {
                                    resDto.IsContinuity = false;
                                }

                                // 进行上架双向绑定
                                reel.StorageLocationId = shelfOn.Id;
                                shelfOn.ReelId = reel.Id;

                                // 保存上架信息 
                                // _repositorysl.Update(shelfOn); 
                                #endregion
                                break;
                            case AllocationType.UpSl: // 下架

                                #region 下架
                                await GetShelfUp();
                                // 清除双向绑定
                                shelfUp.ReelId = null;
                                reel.StorageLocationId = null;
                                LightService.LightOrder(new List<StorageLight>() { new StorageLight()
                                {
                                    ContinuedTime = 10,
                                    LightOrder = 0,
                                    MainBoardId = shelfUp.MainBoardId,
                                    LightColor=shelfUp.LightColor,
                                    RackPositionId = shelfUp.PositionId
                                } });
                                #endregion
                                break;
                            case AllocationType.Send: // 发料

                                #region 发料
                                // 检测发料临时表里面有没有该数据                
                                var sendtemp = await _repositoryRST.FirstOrDefaultAsync(reel.Id);
                                if (sendtemp == null)
                                {
                                    resDto.Msg = "料卷未被挑料";
                                    throw new LYException(resDto.Msg);
                                }
                                // 查询挑料料站表行数据
                                var readySlot = await _repositorySlot.FirstOrDefaultAsync(sendtemp.SlotId);

                                if (sendtemp.IsSend)
                                {
                                    if (readySlot != null)
                                    {
                                        resDto.Msg = @"料卷已经发料    " + sendtemp.FisrtStorageLocationId + "" + "\r\n站位信息: \r\n" + "面别: [" + (readySlot.BoardSide == SideType.B ? "S" : "C") + "]\r\n机器: [" + readySlot.Machine + "]\r\nTable: [" + readySlot.Table + "]\r\n站位: [" +
                                   readySlot.SlotName + "]\r\n边别: [" + (readySlot.Side == SideType.L ? "L" : "R") + "]";
                                    }
                                    else
                                    {
                                        resDto.Msg = "料卷已经发料";
                                    }

                                    throw new LYException(resDto.Msg);
                                }

                                // 查询挑料明细行数据
                                var readyBillD = await _repositoryReadyMBilld.FirstOrDefaultAsync(sendtemp.ReadyMBillDetailedId);

                                // 改变发料状态
                                sendtemp.IsSend = true;

                                // 添加发料数量
                                readyBillD.SendQty += reel.Qty;

                                // 改变料盘备料关联
                                reel.ReadyMBillDetailedId = readyBillD.Id;
                                reel.ReadyMBillId = readyBillD.ReadyMBillId;

                                // 改变物料料站表绑定
                                reel.SlotId = sendtemp.SlotId;

                                // 改变日志备料关联
                                reelMoveLog.ReadyMBillDetailedId = readyBillD.Id;
                                reelMoveLog.ReadyMBillId = readyBillD.ReadyMBillId;
                                reelMoveLog.SlotId = sendtemp.SlotId;

                                // 灭灯
                                await GetShelfUp();
                                shelfUp.LightState = LightState.Off;

                                // 如果发料完成,删除临时表
                                if (_repositoryRST.GetAll().Where(r => r.ReReadyMBillId == sendtemp.ReReadyMBillId && r.IsSend == false).Count() == 1)
                                {
                                    await _repositoryRST.DeleteAsync(r => r.ReReadyMBillId == sendtemp.ReReadyMBillId);
                                }

                                // 查询当前物料的站位信息
                                if (readySlot != null)
                                {
                                    resDto.Msg = "站位信息: \r\n" + "面别: [" + (readySlot.BoardSide == SideType.B ? "S" : "C") + "]\r\n机器: [" + readySlot.Machine + "]\r\nTable: [" + readySlot.Table + "]\r\n站位: [" +
                                                                       readySlot.SlotName + "]\r\n边别: [" + (readySlot.Side == SideType.L ? "L" : "R") + "]";
                                }
                                else
                                {
                                    resDto.Msg = "发料成功";
                                }

                                var readyMs = await _repositoryReadyMBill.GetAll().Where(r => r.ReReadyMBillId == sendtemp.ReReadyMBillId).ToListAsync();

                                var readyMss = readyMs.Select(r => r.Id);

                                var readyMBs = await _repositoryReadyMBilld.GetAll().Where(r => readyMss.Contains(r.ReadyMBillId))
                                    .Select(r => new { r.PartNoId, r.Qty, r.SendQty, r.ReturnQty })
                                    .ToListAsync();

                                if (readyMBs.GroupBy(r => r.PartNoId).Select(r => new { r.Key, Qty = r.Sum(s => s.SendQty) - r.Sum(s => s.Qty) }).Where(r => r.Qty < 0).Count() == 1)
                                {
                                    foreach (var item in readyMs)
                                    {
                                        item.ReadyMStatus = ReadyMStatus.Finish;
                                    }
                                }

                                // 如果为首料进行小车闪灯
                                if (sendtemp.FisrtStorageLocationId != null && sendtemp.FisrtStorageLocationId.Length > 0)
                                {
                                    var shelfF = await _repositorysl.FirstOrDefaultAsync(sendtemp.FisrtStorageLocationId);
                                    StorageLight storage = new StorageLight()
                                    {
                                        ContinuedTime = 10,
                                        LightOrder = 2,
                                        MainBoardId = shelfF.MainBoardId,
                                        RackPositionId = shelfF.PositionId,
                                        LightColor = shelfF.LightColor
                                    };
                                    LightService.LightOrder(new List<StorageLight>() { storage });
                                }

                                // 灭小灯和塔灯
                                LightService.LightOrder(new List<StorageLight>() { new StorageLight()
                                {
                                    ContinuedTime = 10,
                                    LightOrder = 0,
                                    MainBoardId = shelfUp.MainBoardId,
                                    LightColor=shelfUp.LightColor,
                                    RackPositionId = shelfUp.PositionId
                                } });

                                CurrentUnitOfWork.SaveChanges();
                                // 大灯 可能需要修改
                                var lights = _repositorysl.GetAll().Where(s => s.MainBoardId == shelfUp.MainBoardId && s.LightState != LightState.Off && s.LightColor == shelfUp.LightColor);
                                if (lights.Count() == 0)
                                {
                                    LightService.HouseOrder(new List<HouseLight>() { new HouseLight()
                                    {
                                        HouseLightSide =1,
                                        LightOrder = 0,
                                        LightColor =shelfUp.LightColor,
                                        MainBoardId = shelfUp.MainBoardId
                                    } });
                                }

                                #endregion
                                break;
                            case AllocationType.Return: // 退料
                                // 查询最近的发料日志
                                var sendLog = await _repositoryReelMoveLog.GetAll().Where(r => r.IsActive && r.ReelId == reel.Id && r.ReadyMBillDetailedId != null).OrderByDescending(r => r.CreationTime).FirstOrDefaultAsync();

                                if (sendLog != null)
                                {
                                    var readyBillDreturn = await _repositoryReadyMBilld.FirstOrDefaultAsync(sendLog.ReadyMBillDetailedId);
                                    if (sendLog != null)
                                    {
                                        readyBillDreturn.ReturnQty += inputDto.ReturnReelQty;
                                        sendLog.IsActive = false;
                                    }
                                }

                                break;
                            case AllocationType.Received: // 收料

                                #region 收料
                                var receiveId = await _repositoryrrb.FirstOrDefaultAsync(r => r.PartNoId == reel.PartNoId && r.Qty > r.ReceivedQty && r.IQCCheckId == inputDto.IQCCheckId);

                                if (receiveId == null)
                                {
                                    resDto.Msg = "当前ERP没有足够的IQC检验单,请确认";
                                    throw new LYException(resDto.Msg);
                                }

                                receiveId.ReceivedQty += reel.Qty;
                                if (receiveId.ReceivedQty == receiveId.Qty)
                                {
                                    receiveId.IsActive = true;
                                }
                                reel.ReceivedReelBillId = receiveId.Id;
                                reelMoveLog.ReceivedReelBillId = receiveId.Id;
                                #endregion
                                break;
                            case AllocationType.Register: // 注册
                                break;
                            case AllocationType.SendFirstReel: // 发首料
                                break;
                            case AllocationType.SupplyReel: // 补料
                                #region 补料
                                // 检测补料料临时表里面有没有该数据                
                                var supplytemp = await _repositoryReelSupplyTemp.FirstOrDefaultAsync(reel.Id);
                                if (supplytemp == null)
                                {
                                    resDto.Msg = "料卷未被挑料";
                                    throw new LYException(resDto.Msg);
                                }

                                if (supplytemp.IsSend)
                                {
                                    resDto.Msg = @"料卷已经发料    " + supplytemp.FisrtStorageLocationId;
                                    throw new LYException(resDto.Msg);
                                }

                                // 查询挑料明细行数据
                                var readyBillDSupply = await _repositoryReadyMBilld.FirstOrDefaultAsync(supplytemp.ReadyMBillDetailedId);

                                // 改变发料状态
                                supplytemp.IsSend = true;

                                // 添加发料数量
                                readyBillDSupply.SendQty += reel.Qty;

                                // 查询挑料料站表行数据
                                var readySlotSupply = await _repositorySlot.FirstOrDefaultAsync(readyBillDSupply.SlotId);

                                // 改变料盘备料关联
                                reel.ReadyMBillDetailedId = readyBillDSupply.Id;
                                reel.ReadyMBillId = readyBillDSupply.ReadyMBillId;

                                // 改变物料料站表绑定
                                reel.SlotId = supplytemp.SlotId;

                                // 改变日志备料关联
                                reelMoveLog.ReadyMBillDetailedId = readyBillDSupply.Id;
                                reelMoveLog.ReadyMBillId = readyBillDSupply.ReadyMBillId;
                                reelMoveLog.SlotId = supplytemp.SlotId;
                                await GetShelfUp();
                                // 灭灯
                                shelfUp.LightState = LightState.Off;

                                if (readySlotSupply != null)
                                {
                                    resDto.Msg = "站位信息: \r\n" + "面别: [" + (readySlotSupply.BoardSide == SideType.B ? "S" : "C") + "]\r\n机器: [" + readySlotSupply.Machine + "]\r\nTable: [" + readySlotSupply.Table + "]\r\n站位: [" +
                                                                        readySlotSupply.SlotName + "]\r\n边别: [" + (readySlotSupply.Side == SideType.L ? "L" : "R") + "]";
                                }
                                else
                                {
                                    resDto.Msg = "发料成功";
                                }


                                // 灭小灯和塔灯
                                LightService.LightOrder(new List<StorageLight>() { new StorageLight() { ContinuedTime = 10, LightColor = shelfUp.LightColor, LightOrder = 0, MainBoardId = shelfUp.MainBoardId, RackPositionId = shelfUp.PositionId } });

                                CurrentUnitOfWork.SaveChanges();
                                // 大灯 可能需要修改
                                var lightssupply = _repositorysl.GetAll().Where(s => s.MainBoardId == shelfUp.MainBoardId && s.LightState != LightState.Off && s.LightColor == shelfUp.LightColor);
                                if (lightssupply.Count() == 0)
                                {
                                    LightService.HouseOrder(new List<HouseLight>() { new HouseLight()
                                    { HouseLightSide=1, LightOrder = 0, MainBoardId = shelfUp.MainBoardId, LightColor= shelfUp.LightColor } });
                                }

                                #endregion
                                break;
                            case AllocationType.UpByShelf: // 按库位下架
                                break;
                            default:
                                break;
                        }


                        break;
                    case AllocationType.UpByShelf:

                        #region 按库位下架
                        if (inputDto.ShlefLab == null || inputDto.ShlefLab.Length == 0)
                        {
                            inputDto.ShlefLab = inputDto.BarCode;
                        }

                        // 找到库位物料
                        var shelfL = _repositorysl.FirstOrDefault(inputDto.ShlefLab);
                        if (shelfL == null)
                        {
                            throw new LYException(1, "库位[" + inputDto.ShlefLab + "]不存在");
                        }

                        if (shelfL.ReelId == null)
                        {
                            throw new LYException(1, "库位[" + inputDto.ShlefLab + "]上无料");
                        }

                        // 查询库位上 reel
                        reel = _repository.FirstOrDefault(shelfL.ReelId);
                        shelfL.ReelId = null;
                        reel.StorageLocationId = null;
                        reelMoveLog.ReelId = reel.Id;
                        reelMoveLog.PartNoId = reel.PartNoId;
                        reelMoveLog.Qty = reel.Qty;
                        LightService.LightOrder(new List<StorageLight>() { new StorageLight()
                                {
                                    ContinuedTime = 10,
                                    LightOrder = 0,
                                    MainBoardId = shelfL.MainBoardId,
                                    LightColor=shelfL.LightColor,
                                    RackPositionId = shelfL.PositionId
                                } });
                        #endregion
                        break;
                    case AllocationType.Register:

                        #region  料卷注册
                        // 查询料号是否已经维护
                        await BarCodeAnalysis();

                        reel = _repository.FirstOrDefault(reelDto.Id);

                        if (reel == null)
                        {
                            await CheckMPN();
                            // 刚注册物料不能被禁用
                            reelDto.IsActive = true;
                            reel = ObjectMapper.Map<Reel>(reelDto);

                            // 直接进入注册仓库
                            reel.StorageId = reelMpn.RegisterStorageId;
                            reel = await _repository.InsertAsync(reel);


                            // 进行注册,注册后立马保存料盘信息
                            await CurrentUnitOfWork.SaveChangesAsync();
                        }

                        reelMoveLog.ReelId = reel.Id;
                        reelMoveLog.PartNoId = reel.PartNoId;
                        reelMoveLog.Qty = reel.Qty;
                        #endregion
                        break;
                    case AllocationType.Out:
                        #region 出库
                        if (reel == null)     // 进行料卷检查
                        {
                            await CheckReel();
                        }

                        if (reel.StorageLocationId != null)
                        {
                            await GetShelfUp();

                            shelfUp.ReelId = null;
                        }
                        await _repository.DeleteAsync(reel);
                        #endregion
                        break;
                    default:
                        break;
                }


            }
            reelMoveLog.ReelInfo = Newtonsoft.Json.JsonConvert.SerializeObject(reel);
            // 最后插入调拨日志
            await _repositoryReelMoveLog.InsertAsync(reelMoveLog);

            resDto.Reel = ObjectMapper.Map<ReelDto>(reel);

            if (inputDto.ReturnReelQty > 0)
            {
                reel.Qty = inputDto.ReturnReelQty;
                resDto.Reel.Qty = inputDto.ReturnReelQty;
            }

            return resDto;
        }


        [HttpPost]
        public async Task<GetReceivedsResult> GetReceiveds(ReelMoveDto inputDto)
        {
            var res = new GetReceivedsResult() { Msg = "OK", ReceivedReelBills = null };


            // 条码解析
            var reelDtoObj = await _barCodeAnalysisAppService.Analysis(new BaseData.BarCodeAnalysiss.Dto.AnalysisDto() { BarCode = inputDto.BarCode, DtoName = "ReelDto" });
            if (!reelDtoObj.Success)
            {
                res.Msg = reelDtoObj.Msg;
                throw new LYException(res.Msg);
            }
            ReelDto reel = reelDtoObj.Result as ReelDto;

            //所有当前料号未收完成的单据号
            var receiveIdNow = await _repositoryrrb.GetAll().Where(r => r.PartNoId == reel.PartNoId && r.Qty > r.ReceivedQty).ToListAsync();

            if (receiveIdNow == null || receiveIdNow.Count == 0)
            {
                res.Msg = "当前没有足够的收料单,请确认";
                throw new LYException(res.Msg);
            }
            res.ReceivedReelBills = ObjectMapper.Map<List<ReceivedReelBillDto>>(receiveIdNow);
            return res;
        }

        public async Task ReturnReel(ReturnReelDto returnReel)
        {
            // 查询备料单是否存在
            var readyMBill = _repositoryReadyMBill.FirstOrDefault(returnReel.ReadyMBillId);

            if (readyMBill == null)
            {
                throw new LYException(string.Format("备料单{0}不存在", returnReel.ReadyMBillId));
            }

            if (readyMBill.ReReadyMBillId == null)
            {
                throw new LYException(string.Format("备料单{0}未进行备料", returnReel.ReadyMBillId));

            }

            // 查询记账备料单下面是否有该料号
            var readyMBillDetaileds = await _repositoryReadyMBilld.GetAll().Where(r => _repositoryReadyMBill.GetAll()
             .Where(r1 => r1.ReReadyMBillId == readyMBill.ReReadyMBillId)
             .Select(r1 => r1.Id).Contains(r.ReadyMBillId) && r.PartNoId == returnReel.PartNoId).ToListAsync();

            if (readyMBillDetaileds == null || readyMBillDetaileds.Count == 0)
            {
                throw new LYException(string.Format("备料单{0}及其合并的备料单不需要物料{1}", returnReel.ReadyMBillId, returnReel.PartNoId));
            }

            // 查询是否发料
            if (readyMBillDetaileds.Sum(r => r.SendQty) == 0)
            {
                throw new LYException(string.Format("备料单{0}及其合并的备料单物料{1}还未发料", returnReel.ReadyMBillId, returnReel.PartNoId));
            }

            // 进行退料
            var readyMBillDetailed = readyMBillDetaileds.FirstOrDefault();
            readyMBillDetailed.ReturnQty += returnReel.Qty;
        }

        public async Task UpdateReelESL(UpdateReelESLDto updateReelESL)
        {
            foreach (var reelId in updateReelESL.ReelIds)
            {
                var reel = _repository.FirstOrDefault(reelId);
                if (reel != null)
                {
                    reel.ExtendShelfLife += updateReelESL.AddDay;

                    await _repository.UpdateAsync(reel);
                }
            }

        }
        [HttpPost]
        public async Task<ReelMoveResDto> GetIsReturnReel(ReelMoveDto inputDto)
        {
            var res = new ReelMoveResDto();

            // 条码解析
            var reelDtoObj = await _barCodeAnalysisAppService.Analysis(new BaseData.BarCodeAnalysiss.Dto.AnalysisDto() { BarCode = inputDto.BarCode, DtoName = "ReelDto" });
            if (!reelDtoObj.Success)
            {
                res.Msg = reelDtoObj.Msg;
                throw new LYException(res.Msg);
            }
            ReelDto reeldto = reelDtoObj.Result as ReelDto;
            res.Reel = reeldto;
            var reel = await _repositoryReelMoveLog.FirstOrDefaultAsync(r => r.ReelId == reeldto.Id && r.ReadyMBillId != null);

            res.IsContinuity = reel != null;


            return res;
        }
    }
}
