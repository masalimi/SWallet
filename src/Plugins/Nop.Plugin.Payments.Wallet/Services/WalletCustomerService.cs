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
    public class WalletCustomerService : IWalletCustomerService
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

        private readonly IRepository<WalletCustomer> _WalletCustomerRepository;
        private readonly ICacheManager _cacheManager;
        private IWalletCustomerService _walletCustomerServiceImplementation;

        #endregion

        #region Ctor

        #endregion

        public WalletCustomerService(IRepository<WalletCustomer> WalletCustomerRepository,
            ICacheManager cacheManager)
        {
            _WalletCustomerRepository = WalletCustomerRepository;
            _cacheManager = cacheManager;
        }

        public void Log(WalletCustomer record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _WalletCustomerRepository.Insert(record);
        }

        public virtual void InsertWalletCustomer(WalletCustomer WalletCustomer)
        {
            if (WalletCustomer == null)
                throw new ArgumentNullException(nameof(WalletCustomer));

            _WalletCustomerRepository.Insert(WalletCustomer);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }

        //public virtual IPagedList<WalletCustomer> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        //{
        //    var key = string.Format(PAYMENTBYWALLET_ALL_KEY, pageIndex, pageSize);
        //    return _cacheManager.Get(key, () =>
        //    {
        //        var query = from wus in _WalletCustomerRepository.Table
        //                    orderby wus.Name
        //                    select wus;

        //        var records = new PagedList<WalletCustomer>(query, pageIndex, pageSize);
        //        return records;
        //    });
        //}

        //public virtual IPagedList<WalletCustomer> FindRecords(string name, string family, string cardNo, string cvv2,
        //    int pageIndex, int pageSize)
        //{

        //    //filter by weight and shipping method
        //    var wallet = GetAll();


        //    //filter by order subtotal
        //    var nameSearch = !string.IsNullOrEmpty(name) ? wallet.Where(p => p.Name == name) : wallet;

        //    //filter by store
        //    var familySearch = !string.IsNullOrEmpty(family) ? nameSearch.Where(p => p.Family == family) : nameSearch;

        //    //filter by warehouse
        //    var cardoSeach = !string.IsNullOrEmpty(cardNo) ? familySearch.Where(p => p.CardNo == cardNo) : familySearch;

        //    //filter by country
        //    var matchedByCountry = !string.IsNullOrEmpty(cvv2) ? cardoSeach.Where(p => p.Cvv2 == cvv2) : cardoSeach;

        //    ////filter by state/province
        //    //var matchedByStateProvince = stateProvinceId == 0
        //    //    ? matchedByCountry
        //    //    : matchedByCountry.Where(r => r.StateProvinceId == stateProvinceId || r.StateProvinceId == 0);


        //    //sort from particular to general, more particular cases will be the first
        //    var foundRecords = matchedByCountry.OrderBy(r => r.Amount).ThenBy(r => r.CreateDate)
        //        .ThenBy(r => r.Active).ThenBy(r => r.StoreId);

        //    var records = new PagedList<WalletCustomer>(foundRecords.AsQueryable(), pageIndex, pageSize);
        //    return records;
        //}

        public virtual WalletCustomer GetById(int WalletCustomerId)
        {
            if (WalletCustomerId == 0)
                return null;

            return _WalletCustomerRepository.GetById(WalletCustomerId);
        }
        public virtual void UpdateWalletCustomer(WalletCustomer WalletCustomer)
        {
            if (WalletCustomer == null)
                throw new ArgumentNullException(nameof(WalletCustomer));

            _WalletCustomerRepository.Update(WalletCustomer);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }
        public virtual void DeleteWalletCustomer(WalletCustomer WalletCustomer)
        {
            if (WalletCustomer == null)
                throw new ArgumentNullException(nameof(WalletCustomer));

            _WalletCustomerRepository.Delete(WalletCustomer);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }


    }
}
