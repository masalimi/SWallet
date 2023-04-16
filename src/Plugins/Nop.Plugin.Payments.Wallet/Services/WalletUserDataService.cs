using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Payments.Wallet.Data;
using Nop.Plugin.Payments.Wallet.Domain;

namespace Nop.Plugin.Payments.Wallet.Services
{
    public class WalletUserDataService : IWalletUserDataService
    {

        #region Constants

        /// <summary>
        /// Key for caching all records
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        private const string PAYMENTBYWALLET_ALL_KEY = "Nop.paymentbywallet.all-{0}-{1}";

        private const string PAYMENTBYWALLET_PATTERN_KEY = "Nop.paymentbywallet.";

        #endregion

        #region Fields

        private readonly IRepository<WalletUserData> _walletUserDataRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        #endregion

        public WalletUserDataService(IRepository<WalletUserData> walletUserDataRepository,
            ICacheManager cacheManager)
        {
            _walletUserDataRepository = walletUserDataRepository;
            _cacheManager = cacheManager;
        }

        public void Log(WalletUserData record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _walletUserDataRepository.Insert(record);
        }

        public virtual void InsertWalletUserData(WalletUserData walletUserData)
        {
            if (walletUserData == null)
                throw new ArgumentNullException(nameof(walletUserData));

            _walletUserDataRepository.Insert(walletUserData);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }

        public virtual IPagedList<WalletUserData> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(PAYMENTBYWALLET_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from wus in _walletUserDataRepository.Table
                            orderby wus.Name
                            select wus;

                var records = new PagedList<WalletUserData>(query, pageIndex, pageSize);
                return records;
            });
        }

        public virtual IPagedList<WalletUserData> FindRecords(string name, string family, string cardNo, string cvv2,
            int pageIndex, int pageSize)
        {

            //filter by weight and shipping method
            var wallet = GetAll();


            //filter by order subtotal
            var nameSearch = !string.IsNullOrEmpty(name) ? wallet.Where(p => p.Name == name) : wallet;

            //filter by store
            var familySearch = !string.IsNullOrEmpty(family) ? nameSearch.Where(p => p.Family == family) : nameSearch;

            //filter by warehouse
            var cardoSeach = !string.IsNullOrEmpty(cardNo) ? familySearch.Where(p => p.CardNo == cardNo) : familySearch;

            //filter by country
            var matchedByCountry = !string.IsNullOrEmpty(cvv2) ? cardoSeach.Where(p => p.Cvv2 == cvv2) : cardoSeach;

            ////filter by state/province
            //var matchedByStateProvince = stateProvinceId == 0
            //    ? matchedByCountry
            //    : matchedByCountry.Where(r => r.StateProvinceId == stateProvinceId || r.StateProvinceId == 0);


            //sort from particular to general, more particular cases will be the first
            var foundRecords = matchedByCountry.OrderBy(r => r.Name).ThenBy(r => r.Family)
                .ThenBy(r => r.CardNo).ThenBy(r => r.Cvv2);

            var records = new PagedList<WalletUserData>(foundRecords.AsQueryable(), pageIndex, pageSize);
            return records;
        }

        public virtual WalletUserData GetById(int walletUserDataId)
        {
            if (walletUserDataId == 0)
                return null;

            return _walletUserDataRepository.GetById(walletUserDataId);
        }
        public virtual void UpdateWalletUserData(WalletUserData walletUserData)
        {
            if (walletUserData == null)
                throw new ArgumentNullException(nameof(walletUserData));

            _walletUserDataRepository.Update(walletUserData);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }
        public virtual void DeletewalletUserData(WalletUserData walletUserData)
        {
            if (walletUserData == null)
                throw new ArgumentNullException(nameof(walletUserData));

            _walletUserDataRepository.Delete(walletUserData);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }
        public virtual long GetBalanceByUserName(string userName)
        {
            var walletUserData = _walletUserDataRepository.Table.FirstOrDefault(p => p.UserName == userName);
            return walletUserData?.Amount ?? 0;
        }
        public virtual WalletUserData CheckUserIsValid(int customerId)
        {
         //   var walletUserData = _walletUserDataRepository.Table.FirstOrDefault(p => p.CustomerId == customerId);
            return new WalletUserData();// walletUserData;
        }
    }
}
