using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core.Repositories;

namespace AlemanGroupServices.Core
{
    public interface IStationUnitOfWork: IDisposable
    {
        IDataAccess DataAccess { get; }
        //MySQLDBContext DbContext { get; }
        #region Marketing Companies and Warehouses
        IBaseRepository<MarketingCompny> MarketingCompanyRepository { get; }
        IBaseRepository<AccountsInterfaces> AccountsInterfacesRepository { get; }
        IBaseRepository<MarketingCompaniesAccounts> MarketingCompaniesAccountsRepository { get; }
        IBaseRepository<MCAccountProduct> MCAccountProductRepository { get; }
        IBaseRepository<WithdrawalFromMarketingCompany> WithdrawalFromMarketingCompanyRepository { get; }
        IBaseRepository<OrdersQuantity> OrdersQuantityRepository { get; }
        IBaseRepository<ProductDistribution> ProductDistributionRepository { get; }
        #endregion

        #region Subcompanies and Cusotmers
        IBaseRepository<Tblsubcompany> SubcompanyRepository { get; }
        IBaseRepository<TransportationCompany> TransportationCompanyRepository { get; }
        IBaseRepository<CompanyTruck> CompnayTruckRepository { get; }
        IBaseRepository<CompanyDriver> CompanyDriverRepository { get; }
        IBaseRepository<DestinationRegion> DestinationRegionRepository { get; }
        IBaseRepository<Tblcustomer> CustomerRepository { get; }
        IBaseRepository<TblCustomerTurck> CustomersTrucksRepository { get; }
        IBaseRepository<tblCustomersAccounts> CustomersAccountsRepository { get; }
        #endregion

        #region Stations and its Details
        IBaseRepository<Station> StationRepository { get; }

        #region Products Details
        IBaseRepository<Tblmainproduct> MainProductsRepository { get; }
        IBaseRepository<Tblproduct> ProductsRepository { get; }
        IBaseRepository<Tblproductscommission> ProductsCommissionRepository { get; }
        IBaseRepository<Tblproductsbuyprices> ProductsBuyPriceRepository { get; }
        IBaseRepository<Tblproductssalesprice> ProductsSalePriceRepository { get; }
        IBaseRepository<OilSale> OilSaleRepository { get; }
        IBaseRepository<Tblcompaniesofproduct> CompaniesOfProductsRepository { get; }

        #endregion

        #region Tanks Details 
        IBaseRepository<Tbltank> TankRepository { get; }
        IBaseRepository<Tbltankscontentstype> TankcontentTypeRepository { get; }
        IBaseRepository<TblTanksQuantity> TblTanksQuantityRepository { get; }
        IBaseRepository<TanksEquilibrium> TblTanksEquilibriumRepository { get; }
        IBaseRepository<Calibration> CalibrationRepository { get; }
        #endregion

        #region Pumps and counters Details
        IBaseRepository<StationsPumps> PumpsRepository { get; }
        IBaseRepository<PumpsNozels> PumpsNozelsRepository { get; }
        IBaseRepository<CountersPumpsDetail> CountersPumpsDetailRepository { get; }
        IBaseRepository<PumpsTanksDetail> PumpsTanksDetailRepository { get; }
        IBaseRepository<StationsCounter> CountersRepository { get; }
        IBaseRepository<DigitalCountersReads> DigitalCountersReadsRepository { get; }
        IBaseRepository<CountersFeedbackPercentage> CountersFeedbackPercentageRepository { get; }
        #endregion
        #endregion
        int complete();
    }
}
