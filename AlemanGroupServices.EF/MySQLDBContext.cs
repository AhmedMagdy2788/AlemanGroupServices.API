using AlemanGroupServices.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AlemanGroupServices.EF;

//This class is implementing the IDesignTimeDbContextFactory<TContext> interface which is used to create a new instance of a derived DbContext class at design-time. It is used by Entity Framework Core tools (for example, dotnet ef) to create migrations and DbContext instances at design-time. 
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<MySQLDBContext>
{
    // This method creates and returns a new instance of the MySQLDBContext class which is derived from the DbContext class. The method takes an array of strings as an argument which can be used to pass additional parameters to the method. In this case, it is not used. 
    public MySQLDBContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        var optionsBuilder = new DbContextOptionsBuilder<MySQLDBContext>();
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.28-mysql"));
        return new MySQLDBContext(optionsBuilder.Options, configuration);
    }
}

public class MySQLDBContext : DbContext
{
    private readonly IConfiguration _configuration;
    public MySQLDBContext(DbContextOptions<MySQLDBContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    #region Marketing Companies and warhouses
    public DbSet<MarketingCompany> Tblmarketingcompnies { get; set; }
    public DbSet<AccountsInterfaces> Tblaccountsinterfaces { get; set; }
    public DbSet<MarketingCompaniesAccounts> Tblmarketingcompaniesaccounts { get; set; }
    public DbSet<MCAccountProduct> Tblmarketingcompaccountsproducts { get; set; }
    public DbSet<WithdrawalFromMarketingCompany> Tblstationswithdrawals { get; set; }
    public DbSet<OrdersQuantity> Tblordersquantity { get; set; }
    public DbSet<ProductDistribution> Tblproductsdistribution { get; set; }
    public DbSet<Tblsourceregion> Tblsourceregions { get; set; }
    public DbSet<Tblwarehouse> Tblwarehouses { get; set; }
    public DbSet<Tblwarehousecost> Tblwarehousecosts { get; set; }
    #endregion

    #region Subcompanies and Customers
    public DbSet<Subcompany> Tblsubcompanies { get; set; }
    public DbSet<TransportationCompany> Tbltransportationcompanies { get; set; }
    public DbSet<CompanyTruck> Tblstationtrucks { get; set; }
    public DbSet<CompanyDriver> Tblstationdrivers { get; set; }
    public DbSet<DestinationRegion> Tbldestinationregions { get; set; }
    public DbSet<Tblcustomer> Tblcustomers { get; set; }
    public DbSet<TblCustomerTurck> TblcustomersTrucks { get; set; }
    public DbSet<tblCustomersAccounts> TblCustomersAccounts { get; set; }
    #endregion

    #region Stations and its Details
    public DbSet<Station> TblStations { get; set; }
    #region Products Details
    public DbSet<Tblmainproduct> Tblmainproducts { get; set; }
    public DbSet<Tblproduct> TblProducts { get; set; }
    public DbSet<Tblproductscommission> Tblproductscommission { get; set; }
    public DbSet<Tblcompaniesofproduct> Tblcompaniesofproducts { get; set; }
    public DbSet<Tblproductsbuyprices> TblProductsBuyPrice { get; set; }
    public DbSet<Tblproductssalesprice> Tblproductssalesprices { get; set; }
    public DbSet<OilSale> Tbloilssales { get; set; }
    public DbSet<OilsSalesView> Oils_sales_view { get; set; }
    #endregion

    #region Tanks Details
    public DbSet<Tbltank> Tbltanks { get; set; }
    public DbSet<Tbltankscontentstype> Tbltankscontentstype { get; set; }
    public DbSet<TblTanksQuantity> TblTanksQuantity { get; set; }
    public DbSet<TanksEquilibrium> Tbltanksequilibrium
    { get; set; }
    public DbSet<Calibration> Tblcalibration { get; set; }
    #endregion

    #region Pumps and counters
    public DbSet<StationsPumps> tblstationspumps { get; set; }
    public DbSet<PumpsNozels> tblpumpsnozels { get; set; }
    public DbSet<PumpsTanksDetail> tblpumpstanksdetails { get; set; }
    public DbSet<StationsCounter> tblstationscounters { get; set; }
    public DbSet<DigitalCountersReads> tbldigitalcountersreads { get; set; }
    public DbSet<CountersPumpsDetail> tblcounterspumpsdetails { get; set; }
    public DbSet<CountersFeedbackPercentage> tblcountersfeedback { get; set; }

    #endregion
    #endregion

    public void LogToConsole(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        //Console.BackgroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("8.0.28-mysql"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Subcompany>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Tax_Card).HasColumnName("tax_card");
            entity.Property(e => e.Commercial_Registration).HasColumnName("commercial_registration");
            entity.Property(e => e.Fax).HasColumnName("fax");
            entity.Property(e => e.Email).HasColumnName("email");
        });

        modelBuilder.Entity<MarketingCompany>(
            entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Fax).HasColumnName("fax");
                entity.Property(e => e.Email).HasColumnName("email");
            });

        modelBuilder.Entity<TransportationCompany>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransportContractors).HasColumnName("transport_contractors");
        });

        modelBuilder.Entity<CompanyTruck>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyTruckNo).HasColumnName("company_truck_no");
            entity.Property(e => e.Model).HasColumnName("model");
            entity.Property(e => e.TruckLoad).HasColumnName("truck_load");
        });

        modelBuilder.Entity<CompanyDriver>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DriverName).HasColumnName("company_drivers_name");
            entity.Property(e => e.LicenseNo).HasColumnName("license_no");
        });

        modelBuilder.Entity<DestinationRegion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("unloading_regions");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("station_id");
            entity.Property(e => e.Name).HasColumnName("station_name");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Owner_Company_Id).HasColumnName("owner_company_id");
            entity.Property(e => e.Partner_Ship_Id).HasColumnName("partner_ship_id");
            entity.HasOne<Subcompany>()
                .WithMany()
                .HasForeignKey(e => e.Owner_Company_Id).HasConstraintName("tblstations_ibfk_1");
            entity.HasOne<MarketingCompany>()
                .WithMany()
                .HasForeignKey(e => e.Partner_Ship_Id).HasConstraintName("tblstations_ibfk_2");
        });

        modelBuilder.Entity<Tbltank>(entity =>
        {
            entity.HasKey(e => e.Tank_No);
            entity.Property(e => e.Tank_No).HasColumnName("tank_no");
            entity.Property(e => e.Tank_Name).HasColumnName("tank_name");
            entity.Property(e => e.Max_Capacity).HasColumnName("max_capacity");
            entity.Property(e => e.Station_id).HasColumnName("station_id");
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(e => e.Station_id).HasConstraintName("tbltanks_ibfk_1");
        });

        modelBuilder.Entity<Tbltankscontentstype>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Tank_No });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Tank_No).HasColumnName("tank_no");
            entity.Property(e => e.Product).HasColumnName("product");
            entity.HasOne<Tbltank>()
                .WithMany()
                .HasForeignKey(e => e.Tank_No)
                .HasConstraintName("tbltankscontentstype_ibfk_1");
            entity.HasOne<Tblpumptype>()
                .WithMany()
                .HasForeignKey(e => e.Product)
                .HasConstraintName("tbltankscontentstype_ibfk_2");
        });

        modelBuilder.Entity<TblTanksQuantity>(entity =>
        {
            entity.HasKey(e => new { e.Registeration_Date, e.Tank_No });
            // Add additional configuration here
        });

        modelBuilder.Entity<TanksEquilibrium>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Station_Id, e.Product_Name });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Station_Id).HasColumnName("station_id");
            entity.Property(e => e.Product_Name).HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(e => e.Station_Id)
                .HasConstraintName("tbltanksequilibrium_ibfk_1");
            entity.HasOne<Tblpumptype>()
                .WithMany()
                .HasForeignKey(e => e.Product_Name)
                .HasConstraintName("tbltanksequilibrium_ibfk_2");
        });

        modelBuilder.Entity<Calibration>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Station_Id, e.Product_Name });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Station_Id).HasColumnName("station_id");
            entity.Property(e => e.Product_Name).HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(e => e.Station_Id)
                .HasConstraintName("tblcalibration_ibfk_1");
            entity.HasOne<Tblpumptype>()
                .WithMany()
                .HasForeignKey(e => e.Product_Name)
                .HasConstraintName("tblcalibration_ibfk_2");
        });

        modelBuilder.Entity<Tblsourceregion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Warehouse_Region).HasColumnName("warehouse_region");
            // Add additional configuration here
        });

        modelBuilder.Entity<AccountsInterfaces>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.accounts_interfaces).HasColumnName("accounts_interfaces");
            entity.Property(e => e.subcompany_id).HasColumnName("subcompany_id");
            entity.HasOne<Subcompany>()
                .WithMany()
                .HasForeignKey(e => e.subcompany_id)
                .HasConstraintName("tblaccountsinterfaces_fk1");

            entity.HasIndex(a => new { a.subcompany_id, a.accounts_interfaces })
            .IsUnique();
        });

        modelBuilder.Entity<MarketingCompaniesAccounts>(entity =>
        {
            entity.HasKey(e => e.AccountNo);
            entity.Property(e => e.AccountNo).HasColumnName("account_no");
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.AccountInterfaceId).HasColumnName("acount_interface_id");
            entity.Property(e => e.MarketingCompanyId).HasColumnName("marketing_company_id");
            entity.Property(e => e.InitialDept).HasColumnName("initial_dept");
            entity.HasOne<AccountsInterfaces>()
                .WithMany()
                .HasForeignKey(e => e.AccountInterfaceId)
                .HasConstraintName("tblmarketingcompaniesaccounts_fk1");
            entity.HasOne<MarketingCompany>()
                .WithMany()
                .HasForeignKey(e => e.MarketingCompanyId)
                .HasConstraintName("tblmarketingcompaniesaccounts_fk2");
        });

        modelBuilder.Entity<MCAccountProduct>(entity =>
        {
            entity.HasKey(e => new { e.AccountNo, e.MainProductId });
            entity.Property(e => e.AccountNo).HasColumnName("account_no");
            entity.Property(e => e.MainProductId).HasColumnName("main_product_id");
            entity.HasOne<MarketingCompaniesAccounts>()
                .WithMany()
                .HasForeignKey(e => e.AccountNo)
                .HasConstraintName("tblmarketingcompanyaccountsproducts_fk1");
            entity.HasOne<Tblmainproduct>()
                .WithMany()
                .HasForeignKey(e => e.MainProductId)
                .HasConstraintName("tblmarketingcompanyaccountsproducts_fk2");
        });

        modelBuilder.Entity<WithdrawalFromMarketingCompany>(entity =>
        {
            entity.HasKey(e => e.OrderNO);
            entity.Property(e => e.OrderNO).HasColumnName("order_no");
            entity.Property(e => e.OrderDate).HasColumnName("oreder_date");
            entity.Property(e => e.AccountNo).HasColumnName("account_no");
            entity.Property(e => e.Wareahouse).HasColumnName("warehouse");
            entity.Property(e => e.MCInvoiceNo).HasColumnName("marketing_company_invoice_no");
            entity.Property(e => e.WarehouseInvoiceNo).HasColumnName("warehouse_invoice_no");
            entity.Property(e => e.TransportationId).HasColumnName("transportation_id");
            entity.Property(e => e.TruckId).HasColumnName("truck_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.HasOne<MarketingCompaniesAccounts>()
                .WithMany()
                .HasForeignKey(e => e.AccountNo)
                .HasConstraintName("tblstationswithdrawals_fk1");
            entity.HasOne<Tblwarehouse>()
                .WithMany()
                .HasForeignKey(e => e.Wareahouse)
                .HasConstraintName("tblstationswithdrawals_fk2");
            entity.HasOne<TransportationCompany>()
                .WithMany()
                .HasForeignKey(e => e.TransportationId)
                .HasConstraintName("tblstationswithdrawals_fk3");
            entity.HasOne<CompanyTruck>()
                .WithMany()
                .HasForeignKey(e => e.TruckId)
                .HasConstraintName("tblstationswithdrawals_fk4");
            entity.HasOne<CompanyDriver>()
                .WithMany()
                .HasForeignKey(e => e.DriverId)
                .HasConstraintName("tblstationswithdrawals_fk5");
        });

        modelBuilder.Entity<OrdersQuantity>(entity =>
        {
            entity.HasKey(e => new { e.OrderNo, e.ProductId });
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.HasOne<WithdrawalFromMarketingCompany>()
                .WithMany()
                .HasForeignKey(e => e.OrderNo)
                .HasConstraintName("tblordersquantity_fk1");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .HasConstraintName("tblordersquantity_fk2");
        });

        modelBuilder.Entity<ProductDistribution>(entity =>
        {
            entity.HasKey(e => new { e.OrderNo, e.ProductId, e.DestinationId });
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.HasOne<WithdrawalFromMarketingCompany>()
                .WithMany()
                .HasForeignKey(e => e.OrderNo)
                .HasConstraintName("tblproductsdistribution_fk1");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .HasConstraintName("tblproductsdistribution_fk2");
            entity.HasOne<DestinationRegion>()
                .WithMany()
                .HasForeignKey(e => e.DestinationId)
                .HasConstraintName("tblproductsdistribution_fk3");
        });

        modelBuilder.Entity<Tblwarehouse>(entity =>
        {
            entity.HasKey(e => e.Warehouse);
            entity.Property(e => e.Warehouse).HasColumnName("warehouse");
            entity.Property(e => e.Warehouse_Region_Id).HasColumnName("warehouse_region_id");
            entity.Property(e => e.Marketing_Company_Id).HasColumnName("marketing_company_id");
            entity.HasOne<MarketingCompany>()
                .WithMany()
                .HasForeignKey(e => e.Marketing_Company_Id)
                .HasConstraintName("tblwarehouses_ibfk_1");
            entity.HasOne<Tblsourceregion>()
                .WithMany()
                .HasForeignKey(e => e.Warehouse_Region_Id)
                .HasConstraintName("tblwarehouses_ibfk_2");

        });

        modelBuilder.Entity<Tblwarehousecost>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Product_Id, e.Warehouse });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Product_Id).HasColumnName("product_id");
            entity.Property(e => e.Warehouse).HasColumnName("warehouse");
            entity.Property(e => e.Warehouse_Expenses).HasColumnName("warehouse_expenses");
            entity.Property(e => e.Expenses_On_Customer).HasColumnName("expenses_on_customer");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.Product_Id)
                .HasConstraintName("tblwarehousecosts_ibfk_1");
            entity.HasOne<Tblwarehouse>()
                .WithMany()
                .HasForeignKey(e => e.Warehouse)
                .HasConstraintName("tblwarehousecosts_ibfk_2");
        });

        modelBuilder.Entity<Tblmainproduct>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryId);
            entity.Property(e => e.Products_Category);
        });

        modelBuilder.Entity<Tblproduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.Product_Name);
            entity.Property(e => e.Category_Id);
            entity.HasOne<Tblmainproduct>()
                .WithMany()
                .HasForeignKey(e => e.Category_Id);
        });

        modelBuilder.Entity<Tblcompaniesofproduct>(entity =>
        {
            entity.HasKey(e => new { e.Product_Id, e.Source_Company_Id });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Product_Id).HasColumnName("product_id");
            entity.Property(e => e.Source_Company_Id).HasColumnName("source_company_id");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.Product_Id);
            entity.HasOne<MarketingCompany>()
                .WithMany()
                .HasForeignKey(e => e.Source_Company_Id);
            //entity.HasOne(d => d.ProductNavigation)
            //    .WithMany(p => p.Tblproductsbuyprices)
            //    .HasForeignKey(d => d.Product_Id)
            //    .HasConstraintName("tblcompaniesofproducts_ibfk_1");
        });

        modelBuilder.Entity<Tblproductsbuyprices>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Product_Id });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Product_Id).HasColumnName("Product_Id");
            entity.Property(e => e.Product_Purchase_Price).HasColumnName("Product_Purchase_Price");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.Product_Id);
            //entity.HasOne(d => d.ProductNavigation)
            //    .WithMany(p => p.Tblproductsbuyprices)
            //    .HasForeignKey(d => d.Product_Id)
            //    .HasConstraintName("tblproductsbuyprices_ibfk_1");
        });

        modelBuilder.Entity<Tblproductssalesprice>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Product_Id });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Product_Id).HasColumnName("Product_Id");
            entity.Property(e => e.Product_Selling_Price).HasColumnName("Product_Selling_Price");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.Product_Id);
            //entity.HasOne(d => d.ProductNavigation)
            //    .WithMany(p => p.Tblproductsbuyprices)
            //    .HasForeignKey(d => d.Product_Id)
            //    .HasConstraintName("tblproductsbuyprices_ibfk_1");
        });

        modelBuilder.Entity<Tblproductscommission>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Product_Id });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Product_Id).HasColumnName("product_id");
            entity.Property(e => e.Produc_Commission).HasColumnName("product_commission");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.Product_Id);
            //entity.HasOne(d => d.ProductNavigation)
            //    .WithMany(p => p.Tblproductsbuyprices)
            //    .HasForeignKey(d => d.Product_Id)
            //    .HasConstraintName("tblproductsbuyprices_ibfk_1");
        });

        modelBuilder.Entity<OilSale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.OilId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(e => e.StationId)
                .HasConstraintName("tbloilssales_ibfk_1");
            entity.HasOne<Tblproduct>()
                .WithMany()
                .HasForeignKey(e => e.OilId)
                .HasConstraintName("tbloilssales_ibfk_2");
        });

        modelBuilder.Entity<OilsSalesView>(entity =>
        {
            entity.ToView("Oils_sales_view");
            entity.HasNoKey();
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Station_Name).HasColumnName("station_name");
            entity.Property(e => e.Product_Name).HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.product_selling_price).HasColumnName("product_selling_price");
            entity.Property(e => e.Total_Price).HasColumnName("total_price");
        });

        modelBuilder.Entity<StationsPumps>(entity =>
        {
            entity.HasKey(e => e.PumpNo);
            entity.Property(e => e.PumpNo).HasColumnName("pump_no");
            entity.Property(e => e.PumpName).HasColumnName("pump_name");
        });

        modelBuilder.Entity<StationsCounter>(entity =>
        {
            entity.HasKey(e => e.CounterNo);
            entity.Property(e => e.CounterNo).HasColumnName("counter_no");
            entity.Property(e => e.CounterName).HasColumnName("counter_name");
        });

        modelBuilder.Entity<PumpsTanksDetail>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Pump_No });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Pump_No).HasColumnName("pump_no");
            entity.Property(e => e.Tank_No).HasColumnName("tank_no");
            entity.HasOne<StationsPumps>()
                .WithMany()
                .HasForeignKey(e => e.Pump_No)
                .HasConstraintName("tblpumpstanksdetails_ibfk_2");
            entity.HasOne<Tbltank>()
                .WithMany()
                .HasForeignKey(e => e.Tank_No)
                .HasConstraintName("tblpumpstanksdetails_ibfk_1");
        });

        modelBuilder.Entity<PumpsNozels>(entity =>
        {
            entity.HasKey(e => new { e.PumpNo, e.NozelNo });
            entity.Property(e => e.PumpNo).HasColumnName("pump_no");
            entity.Property(e => e.NozelNo).HasColumnName("nozel_no");
            entity.HasOne<StationsPumps>()
                .WithMany()
                .HasForeignKey(e => e.PumpNo)
                .HasConstraintName("tblpumpsnozels_fk1");
        });

        modelBuilder.Entity<CountersPumpsDetail>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.PumpNo, e.NozelNo });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.PumpNo).HasColumnName("pump_no");
            entity.Property(e => e.NozelNo).HasColumnName("nozel_no");
            entity.Property(e => e.CounterNo).HasColumnName("counter_no");
            entity.HasOne<PumpsNozels>()
                .WithMany()
                .HasForeignKey(e => new { e.PumpNo, e.NozelNo })
                .HasConstraintName("tblcounterspumpsdetails_ibfk_1");
            entity.HasOne<StationsCounter>()
                .WithMany()
                .HasForeignKey(e => e.CounterNo)
                .HasConstraintName("tblcounterspumpsdetails_ibfk_2");
        });

        modelBuilder.Entity<DigitalCountersReads>(entity =>
        {
            entity.HasKey(e => new { e.Registeration_Date, e.Counter_No });
            entity.Property(e => e.Registeration_Date).HasColumnName("registeration_date");
            entity.Property(e => e.Counter_No).HasColumnName("counter_no");
            entity.Property(e => e.Counter_Reading).HasColumnName("counter_reading");
            entity.HasOne<StationsCounter>()
                .WithMany()
                .HasForeignKey(e => e.Counter_No)
                .HasConstraintName("tbldigitalcountersreads_ibfk_1");
        });

        modelBuilder.Entity<CountersFeedbackPercentage>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.CounterNo });
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.CounterNo).HasColumnName("counter_no");
            entity.Property(e => e.FeedbackPercentagePerGalon).HasColumnName("feedback_amount_per_galon");
            entity.HasOne<StationsCounter>()
                .WithMany()
                .HasForeignKey(e => e.CounterNo)
                .HasConstraintName("tblcountersfeedback_ibfk_1");
        });
    }
}
