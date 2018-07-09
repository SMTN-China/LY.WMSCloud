using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using AutoMapper;
using LY.WMSCloud.Authorization;
using LY.WMSCloud.CommonService;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.WMS.BaseData.BOMs.Dto;
using LY.WMSCloud.WMS.BaseData.MPNs.Dto;
using LY.WMSCloud.WMS.BaseData.StorageLocations;
using LY.WMSCloud.WMS.BaseData.StorageLocations.Dto;
using LY.WMSCloud.WMS.ProduceData.ReadyMBills.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.Controllers
{
    [Route("api/services/app/[controller]/[action]")]

    public class FileUploadController : WMSCloudControllerBase
    {
        readonly IRepository<BOM, string> _repository;
        readonly FileHelperService _fileHelperService;
        readonly IRepository<ApplicationLanguageText, long> _repositoryI18N;
        readonly IRepository<MPN, String> _repositoryMPN;
        readonly IRepository<Slot, string> _repositorySlot;
        readonly IStorageLocationAppService _storageLocationAppService;
        private IHostingEnvironment _hostingEnv;
        public FileUploadController(IRepository<BOM, string> repository,
            IRepository<Slot, string> repositorySlot,
            IRepository<MPN, String> repositoryMPN,
            FileHelperService fileHelperService,
            IStorageLocationAppService storageLocationAppService,
            IHostingEnvironment env,
            IRepository<ApplicationLanguageText, long> repositoryI18N)
        {
            _repository = repository;
            _fileHelperService = fileHelperService;
            _hostingEnv = env;
            _repositoryI18N = repositoryI18N;
            _repositoryMPN = repositoryMPN;
            _repositorySlot = repositorySlot;
            _storageLocationAppService = storageLocationAppService;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_BOMs)]
        public async Task BOMImport(string productId, IFormFile file)
        {
            try
            {
                var datName = "BOMDto";
                var ps = await GetI18NByDtoName(datName);

                long size = 0;

                var filename = ContentDispositionHeaderValue
                              .Parse(file.ContentDisposition)
                              .FileName
                              .Trim('"');

                filename = _hostingEnv.WebRootPath + $@"\{ filename }";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();

                    var res = await _fileHelperService.ExcleToListEntities<BOMDto>(ps, datName, fs);

                    foreach (var bomDtos in res)
                    {
                        foreach (var bomDto in bomDtos)
                        {
                            bomDto.ProductId = productId;
                            // 查询实体,
                            var bom = _repository.FirstOrDefault(r => r.ProductId == bomDto.ProductId && r.PartNoId == bomDto.PartNoId);
                            if (bom == null)
                            {
                                await _repository.InsertAsync(ObjectMapper.Map<BOM>(bomDto));
                            }
                            else
                            {
                                bom.Qty = bomDto.Qty;
                                bom.Version = bomDto.Version;
                                bom.MoreSendPercentage = bomDto.MoreSendPercentage;
                                bom.AllowableMoreSend = bomDto.AllowableMoreSend;
                            }
                        }
                    }

                }
                System.IO.File.Delete(filename);

            }
            catch (Exception ex)
            {

                throw new LYException(ex.Message);
            }
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_MPNs)]
        public async Task MPNImport(IFormFile file)
        {
            try
            {
                var dtoName = "MPNDto";
                var ps = await GetI18NByDtoName(dtoName);

                long size = 0;

                var filename = ContentDispositionHeaderValue
                              .Parse(file.ContentDisposition)
                              .FileName
                              .Trim('"');

                filename = _hostingEnv.WebRootPath + $@"\{ filename }";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();

                    var res = await _fileHelperService.ExcleToListEntities<MPNDto>(ps, dtoName, fs);

                    foreach (var mpnDtos in res)
                    {
                        foreach (var mpnDto in mpnDtos)
                        {
                            // 查询实体,
                            var mpn = _repositoryMPN.FirstOrDefault(mpnDto.Id);
                            if (mpn == null)
                            {
                                await _repositoryMPN.InsertAsync(ObjectMapper.Map<MPN>(mpnDto));
                            }
                            else
                            {
                                mpn.CustomerId = mpnDto.CustomerId;
                                mpn.IncomingMethod = mpnDto.IncomingMethod;
                                mpn.Info = mpnDto.Info;
                                mpn.IsActive = mpnDto.IsActive;
                                mpn.MPNHierarchy = mpnDto.MPNHierarchy;
                                mpn.MPNLevel = mpnDto.MPNLevel;
                                mpn.MPNType = mpn.MPNType;
                                mpn.MPQs = mpnDto.MPQs;
                                mpn.MSDLevel = mpnDto.MSDLevel;
                                mpn.Supplier = mpnDto.Supplier;
                                mpn.Name = mpnDto.Name;
                                mpn.RegisterStorageId = mpnDto.RegisterStorageId;
                                mpn.Remark = mpnDto.Remark;
                                mpn.ShelfLife = mpnDto.ShelfLife;
                            }
                        }
                    }
                }
                System.IO.File.Delete(filename);
            }
            catch (Exception ex)
            {
                throw new LYException(ex.Message);
            }

        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_ReadyMBills)]
        public async Task<List<ReadyMBillDetailedDto>> ReadyMBillDetailedImport(IFormFile file)
        {
            try
            {
                var dtoName = "ReadyMBillDetailedDto";
                var ps = await GetI18NByDtoName(dtoName);

                long size = 0;

                var filename = ContentDispositionHeaderValue
                              .Parse(file.ContentDisposition)
                              .FileName
                               .Trim('"');

                filename = _hostingEnv.WebRootPath + $@"\{ filename }";
                size += file.Length;
                List<List<ReadyMBillDetailedDto>> res;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();

                    res = await _fileHelperService.ExcleToListEntities<ReadyMBillDetailedDto>(ps, dtoName, fs);
                }

                System.IO.File.Delete(filename);
                return res.Where(s => s.Count > 0).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw new LYException(ex.Message);
            }
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Slots)]
        public async Task SlotImport(string line, IFormFile file)
        {
            try
            {
                List<string> noPns = new List<string>();

                var filename = ContentDispositionHeaderValue
                      .Parse(file.ContentDisposition)
                      .FileName
                       .Trim('"');

                filename = _hostingEnv.WebRootPath + $@"\{ filename }";

                System.IO.File.Delete(filename);
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();


                }
                var lines = System.IO.File.ReadLines(filename).ToArray();

                string machine = null, product = null, table = null, machineType = null, version = null, slotName = null;
                SideType boardSide = SideType.T;
                SideType side = SideType.L;

                List<Slot> listSlot = new List<Slot>();

                if (lines[0].TrimStart().StartsWith("Machine"))  // NXT 料站表
                {
                    machineType = "NXT";
                    foreach (var l in lines)
                    {
                        var strs = l.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                        if (strs.Length == 2 && strs[0] == "Machine") // 获取机器名
                        {
                            machine = strs[1];
                        }
                        if (strs.Length == 2 && strs[0].StartsWith("Recipe")) // 获取机种
                        {
                            var ps = strs[1].Split("_", StringSplitOptions.RemoveEmptyEntries);
                            product = ps[0].Substring(0, ps[0].Length - 1);

                            if (ps[0].Substring(ps[0].Length - 1) == "S")
                            {
                                boardSide = SideType.B;
                            }
                            else
                            {
                                boardSide = SideType.T;
                            }
                        }
                        if (strs.Length == 2 && strs[0] == "Revision") // 获取版本
                        {
                            version = strs[1];
                        }



                        if (strs.Length == 7 && strs[0].Contains("- "))
                        {

                            var partNoId = strs[1].Trim();

                            if (partNoId != null && _repositoryMPN.FirstOrDefault(partNoId) != null)
                            {
                                if (int.Parse(strs[5]) == 0)
                                {
                                    continue;
                                }
                                table = strs[0].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Replace("-", "");

                                var slot = _repositorySlot.FirstOrDefault(s => s.BoardSide == boardSide && s.Machine == machine && s.ProductId == product && s.LineId == line && s.Table == table && s.PartNoId == partNoId && s.SlotName == strs[0]);
                                if (slot == null)
                                {
                                    listSlot.Add(new Slot()
                                    {
                                        BoardSide = boardSide,
                                        Feeder = strs[4],
                                        IsActive = true,
                                        LineId = line,
                                        Machine = machine,
                                        PartNoId = partNoId,
                                        Version = version,
                                        Table = table,
                                        SlotName = strs[0],
                                        Side = SideType.L,
                                        Qty = int.Parse(strs[5]),
                                        ProductId = product,
                                        MachineType = machineType,
                                        Location = string.Join(",", strs[6].Trim().Replace("\"", "").Replace("-", "").Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries).Length > 1 ?
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[1] :
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[0]).Distinct().OrderBy(s => s)),
                                        LineSide = SideType.L,

                                    });
                                }
                                else
                                {
                                    slot.BoardSide = boardSide;
                                    slot.Feeder = strs[4];
                                    slot.IsActive = true;
                                    slot.LineId = line;
                                    slot.Machine = machine;
                                    slot.PartNoId = partNoId;
                                    slot.Version = version;
                                    slot.Table = table;
                                    slot.SlotName = strs[0];
                                    slot.Side = SideType.L;
                                    slot.Qty = int.Parse(strs[5]);
                                    slot.ProductId = product;
                                    slot.MachineType = machineType;
                                    slot.Location = string.Join(",", strs[6].Trim().Replace("\"", "").Replace("-", "").Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries).Length > 1 ?
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[1] :
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[0]).OrderBy(s => s));
                                    slot.LineSide = SideType.L;
                                    listSlot.Add(slot);
                                }
                            }

                        }

                        if (strs.Length == 6 && strs[0].Contains("- "))
                        {

                            var partNoId = strs[1].Trim();

                            if (int.Parse(strs[4]) == 0)
                            {
                                continue;
                            }

                            if (partNoId != null && _repositoryMPN.FirstOrDefault(partNoId) != null)
                            {
                                table = strs[0].Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Replace("-", "");

                                var slot = _repositorySlot.FirstOrDefault(s => s.BoardSide == boardSide && s.Machine == machine && s.ProductId == product && s.LineId == line && s.Table == table && s.PartNoId == partNoId && s.SlotName == strs[0]);
                                if (slot == null)
                                {
                                    listSlot.Add(new Slot()
                                    {
                                        BoardSide = boardSide,
                                        IsActive = true,
                                        LineId = line,
                                        Machine = machine,
                                        PartNoId = partNoId,
                                        Version = version,
                                        Table = table,
                                        SlotName = strs[0],
                                        Side = SideType.L,
                                        Qty = int.Parse(strs[4]),
                                        ProductId = product,
                                        MachineType = machineType,
                                        Location = string.Join(",", strs[5].Trim().Replace("\"", "").Replace("-", "").Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries).Length > 1 ?
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[1] :
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[0]).OrderBy(s => s)),
                                        LineSide = SideType.L,
                                    });
                                }
                                else
                                {
                                    slot.BoardSide = boardSide;
                                    slot.IsActive = true;
                                    slot.LineId = line;
                                    slot.Machine = machine;
                                    slot.PartNoId = partNoId;
                                    slot.Version = version;
                                    slot.Table = table;
                                    slot.SlotName = strs[0];
                                    slot.Side = SideType.L;
                                    slot.Qty = int.Parse(strs[4]);
                                    slot.ProductId = product;
                                    slot.MachineType = machineType;
                                    slot.Location = string.Join(",", strs[5].Trim().Replace("\"", "").Replace("-", "").Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries).Length > 1 ?
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[1] :
                                        s.Trim().Split(':', StringSplitOptions.RemoveEmptyEntries)[0]).OrderBy(s => s));
                                    slot.LineSide = SideType.L;
                                    listSlot.Add(slot);
                                }

                            }
                        }
                    }
                }

                if (lines[0].TrimStart().Contains("Lot"))  // CM 料站表
                {
                    machineType = "CM";
                    DataTable dataTable = _fileHelperService.OpenCSV(filename);

                    foreach (DataRow item in dataTable.Rows)
                    {
                        if (item[0].ToString().Contains("Lot")) // 
                        {
                            var strs = item[0].ToString().Split(":");

                            var ps = strs[1].Split(new char[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
                            product = ps[0].Substring(0, ps[0].Length - 1).Trim();
                            version = ps[1].Trim();
                            if (ps[0].Substring(ps[0].Length - 1).Trim() == "S")
                            {
                                boardSide = SideType.B;
                            }
                            else
                            {
                                boardSide = SideType.T;
                            }
                        }

                        if (item[0].ToString().Contains("MC") && !item[0].ToString().Contains("File"))
                        {
                            machine = item[0].ToString().Split(":", StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                        var partNoId = item[2].ToString().Trim();
                        // 查询料号是否维护
                        if (partNoId != null && _repositoryMPN.FirstOrDefault(partNoId) != null)
                        {
                            if (int.Parse(item[7].ToString().Trim()) == 0)
                            {
                                continue;
                            }

                            // 变更站位
                            if (item[1].ToString().Trim().Length > 0)
                            {
                                slotName = item[1].ToString().Trim();
                            }

                            table = item[0].ToString().Trim();
                            if (item[4].ToString().Trim() == "R")
                            {
                                side = SideType.R;
                            }
                            else
                            {
                                side = SideType.L;
                            }

                            var slot = _repositorySlot.FirstOrDefault(s => s.BoardSide == boardSide && s.Machine == machine && s.ProductId == product && s.LineId == line && s.Table == table && s.PartNoId == partNoId && s.SlotName == slotName && s.Side == side);
                            if (slot == null)
                            {
                                listSlot.Add(new Slot()
                                {
                                    BoardSide = boardSide,
                                    Feeder = item[5].ToString().Trim(),
                                    IsActive = true,
                                    LineId = line,
                                    Machine = machine,
                                    PartNoId = partNoId,
                                    Version = version,
                                    Table = table,
                                    SlotName = slotName,
                                    Side = side,
                                    Qty = int.Parse(item[7].ToString().Trim()),
                                    ProductId = product,
                                    MachineType = machineType,
                                    Location = string.Join(",", item[8].ToString().Trim().Replace("\"", "").Replace("-", "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s)),
                                    LineSide = SideType.L,

                                });
                            }
                            else
                            {
                                slot.BoardSide = boardSide;
                                slot.Feeder = item[5].ToString().Trim();
                                slot.IsActive = true;
                                slot.LineId = line;
                                slot.Machine = machine;
                                slot.PartNoId = partNoId;
                                slot.Version = version;
                                slot.Table = table;
                                slot.SlotName = slotName;
                                slot.Side = side;
                                slot.Qty = int.Parse(item[7].ToString().Trim());
                                slot.ProductId = product;
                                slot.MachineType = machineType;
                                slot.Location = string.Join(",", item[8].ToString().Trim().Replace("\"", "").Replace("-", "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s));
                                slot.LineSide = SideType.L;
                                listSlot.Add(slot);
                            }
                        }
                    }
                }

                if (lines[0].TrimStart().StartsWith("Recipe"))  // NPM 料站表
                {
                    machineType = "NPM";
                    DataTable dataTable = _fileHelperService.OpenCSV(filename);
                    foreach (DataRow item in dataTable.Rows)
                    {
                        if (item[0].ToString().Contains("Recipe")) // 
                        {
                            var strs = item[0].ToString().Split(":");

                            var ps = strs[1].Split("_", StringSplitOptions.RemoveEmptyEntries);
                            product = ps[0].Substring(0, ps[0].Length - 1).Trim();
                            version = ps[1].Trim();
                            if (ps[0].Substring(ps[0].Length - 1).Trim() == "S")
                            {
                                boardSide = SideType.B;
                            }
                            else
                            {
                                boardSide = SideType.T;
                            }
                        }

                        var partNoId = item[5].ToString().Trim();
                        // 查询料号是否维护
                        if (partNoId != null && _repositoryMPN.FirstOrDefault(partNoId) != null)
                        {
                            if (int.Parse(item[7].ToString().Trim()) == 0)
                            {
                                continue;
                            }

                            // 变更站位
                            if (item[3].ToString().Trim().Length > 0)
                            {
                                slotName = item[3].ToString().Trim();
                            }

                            table = item[2].ToString().Trim();
                            if (item[4].ToString().Trim() == "R")
                            {
                                side = SideType.R;
                            }
                            else
                            {
                                side = SideType.L;
                            }

                            machine = item[1].ToString().Trim();

                            var slot = _repositorySlot.FirstOrDefault(s => s.BoardSide == boardSide && s.Machine == machine && s.ProductId == product && s.LineId == line && s.Table == table && s.PartNoId == partNoId && s.SlotName == slotName && s.Side == side);
                            if (slot == null)
                            {
                                listSlot.Add(new Slot()
                                {
                                    BoardSide = boardSide,
                                    Feeder = item[6].ToString().Trim(),
                                    IsActive = true,
                                    LineId = line,
                                    Machine = machine,
                                    PartNoId = partNoId,
                                    Version = version,
                                    Table = table,
                                    SlotName = slotName,
                                    Side = side,
                                    Qty = int.Parse(item[7].ToString().Trim()),
                                    ProductId = product,
                                    MachineType = machineType,
                                    Location = string.Join(",", item[8].ToString().Trim().Replace("\"", "").Replace("-", "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s)),
                                    LineSide = SideType.L,

                                });
                            }
                            else
                            {
                                slot.BoardSide = boardSide;
                                slot.Feeder = item[6].ToString().Trim();
                                slot.IsActive = true;
                                slot.LineId = line;
                                slot.Machine = machine;
                                slot.PartNoId = partNoId;
                                slot.Version = version;
                                slot.Table = table;
                                slot.SlotName = slotName;
                                slot.Side = side;
                                slot.Qty = int.Parse(item[7].ToString().Trim());
                                slot.ProductId = product;
                                slot.MachineType = machineType;
                                slot.Location = string.Join(",", item[8].ToString().Trim().Replace("\"", "").Replace("-", "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s));
                                slot.LineSide = SideType.L;
                                listSlot.Add(slot);
                            }
                        }
                        else
                        {
                            if (item[5].ToString().Trim().Length > 0)
                            {
                                noPns.Add(item[5].ToString().Trim());
                            }

                        }
                    }


                }
                int slotIndex = 1;
                foreach (var item in listSlot)
                {
                    item.Index = slotIndex;
                    slotIndex++;
                    await _repositorySlot.InsertOrUpdateAsync(item);
                }
                CurrentUnitOfWork.SaveChanges();

                // 将不在本次料表中的站位更新为无效
                // 查询该机种该线别该版本
                var NoActives = _repositorySlot.GetAll().Where(r => r.BoardSide == listSlot.FirstOrDefault().BoardSide && r.ProductId == listSlot.FirstOrDefault().ProductId && r.LineId == listSlot.FirstOrDefault().LineId && !listSlot.Select(s => s.Id).Contains(r.Id)).ToArray();

                foreach (var item in NoActives)
                {
                    item.IsActive = false;
                    await _repositorySlot.UpdateAsync(item);
                }
                System.IO.File.Delete(filename);
            }
            catch (Exception ex)
            {

                throw new LYException(ex.Message);
            }
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_StorageLocations)]
        public async Task StorageLocationsImport(IFormFile file)
        {
            try
            {
                var dtoName = "StorageLocationDto";
                var ps = await GetI18NByDtoName(dtoName);

                long size = 0;

                var filename = ContentDispositionHeaderValue
                              .Parse(file.ContentDisposition)
                              .FileName
                               .Trim('"');

                filename = _hostingEnv.WebRootPath + $@"\{ filename }";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();

                    var res = await _fileHelperService.ExcleToListEntities<StorageLocationDto>(ps, dtoName, fs);

                    foreach (var item in res)
                    {
                        foreach (var sl in item)
                        {
                            var isHave = await _storageLocationAppService.GetIsHave(sl.Id);
                            if (isHave)
                            {
                                await _storageLocationAppService.Update(sl);
                            }
                            else
                            {
                                await _storageLocationAppService.Create(sl);
                            }
                        }
                    }
                }
                System.IO.File.Delete(filename);
            }
            catch (Exception ex)
            {

                throw new LYException(ex.Message);
            }
        }

        async Task<List<ApplicationLanguageText>> GetI18NByDtoName(String dtoName)
        {

            var thisAssembly = typeof(WMSCloudApplicationModule).GetAssembly().ExportedTypes.Where(t => t.Name.ToLower() == dtoName.ToLower()).FirstOrDefault();

            // 获取当前语言
            var lang = await SettingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, AbpSession.ToUserIdentifier());

            var dbI18N = await _repositoryI18N.GetAll().Where(i => i.Key.StartsWith(dtoName) && i.LanguageName == lang).ToListAsync();

            var p = thisAssembly.GetProperties();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<System.Reflection.PropertyInfo, ApplicationLanguageText>()
                    .ForMember(m => m.Key, opt => opt.MapFrom(s => dtoName + s.Name))
                    .ForMember(m => m.Value, opt => opt.MapFrom(s =>
                        dbI18N.FirstOrDefault(i => i.Key == dtoName + s.Name) == null ? s.Name : dbI18N.FirstOrDefault(i => i.Key == dtoName + s.Name).Value))
                    .ForMember(m => m.LanguageName, opt => opt.MapFrom(s => lang));
            }
             );

            return config.CreateMapper().Map<List<System.Reflection.PropertyInfo>, List<ApplicationLanguageText>>(p.ToList());
        }
    }
}
