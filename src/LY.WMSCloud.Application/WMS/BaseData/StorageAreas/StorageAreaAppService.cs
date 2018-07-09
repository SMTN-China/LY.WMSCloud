using Abp.Application.Services.Dto;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.WMS.BaseData.StorageAreas.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMSCloud.WMS.BaseData.StorageAreas
{
    public class StorageAreaAppService : ServiceBase<StorageArea, StorageAreaDto, string>, IStorageAreaAppService
    {
        readonly IWMSRepositories<StorageArea, string> _repository;
        readonly IWMSRepositories<MPN, string> _repositoryMPN;
        readonly IWMSRepositories<MPNStorageAreaMap, string> _repositoryMPNSM;
        readonly IWMSRepositories<StorageLocation, string> _repositorySL;

        public StorageAreaAppService(
            IWMSRepositories<StorageArea, string> repository,
            IWMSRepositories<MPN, string> repositoryMPN,
            IWMSRepositories<MPNStorageAreaMap, string> repositoryMPNSM,
            IWMSRepositories<StorageLocation, string> repositorySL
            ) : base(repository)
        {
            _repository = repository;
            _repositoryMPN = repositoryMPN;
            _repositoryMPNSM = repositoryMPNSM;
            _repositorySL = repositorySL;
        }

        public async override Task<StorageAreaDto> Create(StorageAreaDto input)
        {

            var StorageArea = await base.Create(input);

            CurrentUnitOfWork.SaveChanges();


            // 更新库位区域
            var StorageLocations = _repositorySL.GetAll().Where(r => input.ShelfNames.Contains(r.Name)).ToArray();

            foreach (var StorageLocation in StorageLocations)
            {
                StorageLocation.StorageAreaId = StorageArea.Id;
            }

            // 更新料号区域
            var MPNS = _repositoryMPN.GetAll().Where(r => input.MPNIds.Contains(r.Id)).ToArray();

            foreach (var mpn in MPNS)
            {
                _repositoryMPNSM.Insert(new MPNStorageAreaMap() { MPNId = mpn.Id, StorageAreaId = StorageArea.Id });
            }

            return StorageArea;
        }

        public async override Task<StorageAreaDto> Update(StorageAreaDto input)
        {
            // 删除区域现有货架
            var OldStorageLocations = _repositorySL.GetAll().Where(r => r.StorageAreaId == input.Id).ToArray();
            foreach (var StorageLocation in OldStorageLocations)
            {
                StorageLocation.StorageAreaId = null;
            }

            // 清空区域现有物料
            var MPNStorageAreaMaps = _repositoryMPNSM.GetAll().Where(r => r.StorageAreaId == input.Id).ToArray();
            foreach (var MPNStorageAreaMap in MPNStorageAreaMaps)
            {
                _repositoryMPNSM.Delete(MPNStorageAreaMap);
            }

            CurrentUnitOfWork.SaveChanges();

            var StorageArea = await base.Update(input);

            CurrentUnitOfWork.SaveChanges();

            // 更新库位区域
            var StorageLocations = _repositorySL.GetAll().Where(r => input.ShelfNames.Contains(r.Name)).ToArray();

            foreach (var StorageLocation in StorageLocations)
            {
                StorageLocation.StorageAreaId = StorageArea.Id;
            }

            // 更新料号区域
            var MPNS = _repositoryMPN.GetAll().Where(r => input.MPNIds.Contains(r.Name)).ToArray();

            foreach (var mpn in MPNS)
            {
                _repositoryMPNSM.Insert(new MPNStorageAreaMap() { MPNId = mpn.Id, StorageAreaId = StorageArea.Id });
            }

            return StorageArea;
        }


        public override Task Delete(EntityDto<string> input)
        {
            // 删除区域现有货架
            var OldStorageLocations = _repositorySL.GetAll().Where(r => r.StorageAreaId == input.Id).ToArray();
            foreach (var StorageLocation in OldStorageLocations)
            {
                StorageLocation.StorageAreaId = null;
            }

            // 清空区域现有物料
            var MPNStorageAreaMaps = _repositoryMPNSM.GetAll().Where(r => r.StorageAreaId == input.Id).ToArray();
            foreach (var MPNStorageAreaMap in MPNStorageAreaMaps)
            {
                _repositoryMPNSM.Delete(MPNStorageAreaMap);
            }

            CurrentUnitOfWork.SaveChanges();


            return base.Delete(input);
        }

        public async Task<ICollection<string>> GetShelfByKeyName(string keyName)
        {
            var res = await _repositorySL.GetAll().Where(r => r.StorageAreaId == null).GroupBy(r => r.Name).Select(r => r.Key).Where(r => r.Contains(keyName)).Take(10).ToListAsync();

            return res;
        }
    }
}
