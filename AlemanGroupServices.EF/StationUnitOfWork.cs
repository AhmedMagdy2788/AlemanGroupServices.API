using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core.Repositories;
using AlemanGroupServices.EF.Repositories;

namespace AlemanGroupServices.EF
{
    public class StationUnitOfWork : IStationUnitOfWork
    {
        private readonly MySQLDBContext _dbContext;
        public IDataAccess dataAccess;
        public StationUnitOfWork(MySQLDBContext dbContext, IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            _dbContext = dbContext;
            #region Marketing Companies
            MarketingCompanyRepository =
                new BaseRepository<MarketingCompny>(_dbContext);
            AccountsInterfacesRepository =
                new BaseRepository<AccountsInterfaces>(_dbContext);
            MarketingCompaniesAccountsRepository =
                new BaseRepository<MarketingCompaniesAccounts>(_dbContext);
            MCAccountProductRepository =
                new BaseRepository<MCAccountProduct>(_dbContext);
            WithdrawalFromMarketingCompanyRepository =
                new BaseRepository<WithdrawalFromMarketingCompany>(_dbContext);
            #endregion
            #region Subcopanies and Customers
            SubcompanyRepository =
                new BaseRepository<Tblsubcompany>(_dbContext);
            TransportationCompanyRepository =
                new BaseRepository<TransportationCompany>(_dbContext);
            CompnayTruckRepository =
                new BaseRepository<CompanyTruck>(_dbContext);
            CompanyDriverRepository =
                new BaseRepository<CompanyDriver>(_dbContext);
            CustomerRepository =
                new BaseRepository<Tblcustomer>(_dbContext);
            CustomersAccountsRepository =
                new BaseRepository<tblCustomersAccounts>(_dbContext);
            CustomersTrucksRepository =
                new BaseRepository<TblCustomerTurck>(_dbContext);
            #endregion

            #region Station Details 
            StationRepository =
                new BaseRepository<Station>(_dbContext);

            #region Products Details
            MainProductsRepository =
                new BaseRepository<Tblmainproduct>(_dbContext);
            ProductsRepository =
                new BaseRepository<Tblproduct>(_dbContext);
            ProductsBuyPriceRepository =
                new BaseRepository<Tblproductsbuyprices>(_dbContext);
            ProductsSalePriceRepository =
                new BaseRepository<Tblproductssalesprice>(_dbContext);
            ProductsCommissionRepository =
                new BaseRepository<Tblproductscommission>(_dbContext);
            CompaniesOfProductsRepository =
                new BaseRepository<Tblcompaniesofproduct>(_dbContext);
            #endregion

            #region Tanks Details
            TankRepository =
                new BaseRepository<Tbltank>(_dbContext);
            TankcontentTypeRepository =
                new BaseRepository<Tbltankscontentstype>(_dbContext);
            TblTanksQuantityRepository =
                new BaseRepository<TblTanksQuantity>(_dbContext);
            TblTanksEquilibriumRepository =
                new BaseRepository<TanksEquilibrium>(_dbContext);
            CalibrationRepository =
                new BaseRepository<Calibration>(_dbContext);
            #endregion

            #region Pumps and Counters
            PumpsRepository =
                new BaseRepository<StationsPumps>(_dbContext);
            PumpsNozelsRepository =
                new BaseRepository<PumpsNozels>(_dbContext);
            CountersPumpsDetailRepository =
                new BaseRepository<CountersPumpsDetail>(_dbContext);
            PumpsTanksDetailRepository =
                new BaseRepository<PumpsTanksDetail>(_dbContext);
            CountersRepository =
                new BaseRepository<StationsCounter>(_dbContext);
            DigitalCountersReadsRepository =
                new BaseRepository<DigitalCountersReads>(_dbContext);
            CountersFeedbackPercentageRepository =
                new BaseRepository<CountersFeedbackPercentage>(_dbContext);
            #endregion
            #endregion
        }

        public IBaseRepository<Tblsubcompany> SubcompanyRepository { get; private set; }
        public IBaseRepository<TransportationCompany> TransportationCompanyRepository { get; private set; }
        public IBaseRepository<CompanyTruck> CompnayTruckRepository { get; private set; }
        public IBaseRepository<CompanyDriver> CompanyDriverRepository { get; private set; }

        public IBaseRepository<MarketingCompny> MarketingCompanyRepository { get; private set; }
        public IBaseRepository<AccountsInterfaces> AccountsInterfacesRepository { get; private set; }
        public IBaseRepository<MarketingCompaniesAccounts> MarketingCompaniesAccountsRepository { get; private set; }
        public IBaseRepository<MCAccountProduct> MCAccountProductRepository { get; private set; }
        public IBaseRepository<WithdrawalFromMarketingCompany> WithdrawalFromMarketingCompanyRepository { get; private set; }

        public IBaseRepository<Station> StationRepository { get; private set; }
        public IBaseRepository<Tbltank> TankRepository { get; private set; }
        public IBaseRepository<Tbltankscontentstype> TankcontentTypeRepository { get; private set; }
        public IBaseRepository<TblTanksQuantity> TblTanksQuantityRepository { get; private set; }
        public IBaseRepository<TanksEquilibrium> TblTanksEquilibriumRepository { get; private set; }
        public IBaseRepository<Calibration> CalibrationRepository { get; private set; }
        public IBaseRepository<DigitalCountersReads> DigitalCountersReadsRepository { get; private set; }
        public IBaseRepository<CountersFeedbackPercentage> CountersFeedbackPercentageRepository { get; private set; }

        public IBaseRepository<Tblcustomer> CustomerRepository { get; private set; }
        public IBaseRepository<StationsPumps> PumpsRepository { get; private set; }
        public IBaseRepository<PumpsNozels> PumpsNozelsRepository { get; private set; }
        public IBaseRepository<CountersPumpsDetail> CountersPumpsDetailRepository { get; private set; }
        public IBaseRepository<StationsCounter> CountersRepository { get; private set; }
        public IBaseRepository<PumpsTanksDetail> PumpsTanksDetailRepository { get; private set; }
        public IBaseRepository<TblCustomerTurck> CustomersTrucksRepository { get; private set; }

        public IBaseRepository<tblCustomersAccounts> CustomersAccountsRepository { get; private set; }

        public IBaseRepository<Tblmainproduct> MainProductsRepository { get; private set; }

        public IBaseRepository<Tblproduct> ProductsRepository { get; private set; }

        public IBaseRepository<Tblproductscommission> ProductsCommissionRepository { get; private set; }

        public IBaseRepository<Tblcompaniesofproduct> CompaniesOfProductsRepository { get; private set; }

        public IBaseRepository<Tblproductsbuyprices> ProductsBuyPriceRepository { get; private set; }

        public IBaseRepository<Tblproductssalesprice> ProductsSalePriceRepository { get; private set; }

        public IDataAccess DataAccess => dataAccess;

        public int complete()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
