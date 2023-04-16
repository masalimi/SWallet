using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Payments.Wallet.Data;
using Nop.Plugin.Payments.Wallet.Domain;
using Nop.Plugin.Payments.Wallet.Models;

namespace Nop.Plugin.Payments.Wallet.Services
{
    public class WalletCustomerHistoryService : IWalletCustomerHistoryService
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

        private readonly IRepository<WalletCustomerHistory> _WalletCustomerHistoryRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<WalletCustomer> _WalletCustomerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Store> _storeRepository;


        #endregion

        #region Ctor

        #endregion

        public WalletCustomerHistoryService(IRepository<WalletCustomerHistory> WalletCustomerHistoryRepository,
            ICacheManager cacheManager, IRepository<WalletCustomer> walletCustomerRepository, IRepository<Customer> customerRepository, IRepository<Order> orderRepository,
            IRepository<Store> storeRepository)
        {
            _WalletCustomerHistoryRepository = WalletCustomerHistoryRepository;
            _cacheManager = cacheManager;
            _WalletCustomerRepository = walletCustomerRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
        }

        public void Log(WalletCustomerHistory record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _WalletCustomerHistoryRepository.Insert(record);
        }

        public virtual void InsertWalletCustomerHistory(WalletCustomerHistory WalletCustomerHistory)
        {
            if (WalletCustomerHistory == null)
                throw new ArgumentNullException(nameof(WalletCustomerHistory));

            _WalletCustomerHistoryRepository.Insert(WalletCustomerHistory);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }

        public virtual IPagedList<WalletCustomerHistoryModel> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(PAYMENTBYWALLET_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from wus in _WalletCustomerHistoryRepository.Table
                            join ord in _orderRepository.Table on wus.OrderId equals ord.Id
                            join wcus in _WalletCustomerRepository.Table on wus.WalletCustomerId equals wcus.Id
                            join cus in _customerRepository.Table on wcus.CustomerId equals cus.Id
                            join str in _storeRepository.Table on wus.StoreId equals str.Id
                            orderby cus.Username
                            select new WalletCustomerHistoryModel()
                            {
                                Amount = wcus.Amount,
                                CreateDate = wcus.CreateDate,
                                OrderNo = ord.CustomOrderNumber,
                                TransferTypeWallet = wus.TransferType
                            };

                var records = new PagedList<WalletCustomerHistoryModel>(query, pageIndex, pageSize);
                return records;
            });
        }

        public virtual WalletCustomerHistory GetById(int WalletCustomerHistoryId)
        {
            if (WalletCustomerHistoryId == 0)
                return null;

            return _WalletCustomerHistoryRepository.GetById(WalletCustomerHistoryId);
        }
        public virtual void UpdateWalletCustomerHistory(WalletCustomerHistory WalletCustomerHistory)
        {
            if (WalletCustomerHistory == null)
                throw new ArgumentNullException(nameof(WalletCustomerHistory));

            _WalletCustomerHistoryRepository.Update(WalletCustomerHistory);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }
        public virtual void DeleteWalletCustomerHistory(WalletCustomerHistory WalletCustomerHistory)
        {
            if (WalletCustomerHistory == null)
                throw new ArgumentNullException(nameof(WalletCustomerHistory));

            _WalletCustomerHistoryRepository.Delete(WalletCustomerHistory);

            _cacheManager.RemoveByPrefix(PAYMENTBYWALLET_PATTERN_KEY);
        }


    }
}
