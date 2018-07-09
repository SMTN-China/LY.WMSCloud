using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using LY.WMSCloud.Authorization.Roles;
using LY.WMSCloud.Authorization.Users;
using LY.WMSCloud.MultiTenancy;
using LY.WMSCloud.Entities;
using LY.WMSCloud.Entities.BaseData;
using LY.WMSCloud.Entities.StorageData;
using LY.WMSCloud.Entities.ProduceData;

namespace LY.WMSCloud.EntityFrameworkCore
{
    public class WMSCloudDbContext : AbpZeroDbContext<Tenant, Role, User, WMSCloudDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<Org> Orgs { set; get; }

        public virtual DbSet<Menu> Menus { set; get; }

        public virtual DbSet<BarCodeAnalysis> BarCodeAnalysiss { set; get; }
        public virtual DbSet<BOM> BOMs { set; get; }
        public virtual DbSet<Customer> Customers { set; get; }
        public virtual DbSet<Line> Lines { set; get; }
        public virtual DbSet<MPN> MPNs { set; get; }
        public virtual DbSet<StorageLocation> StorageLocations { set; get; }
        public virtual DbSet<StorageLocationType> StorageLocationTypes { set; get; }

        public virtual DbSet<UPH> UPHs { set; get; }

        public virtual DbSet<Slot> Slots { set; get; }
        public virtual DbSet<Storage> Storages { set; get; }
        public virtual DbSet<Reel> Reels { set; get; }
        public virtual DbSet<ReelSendTemp> ReelSendTemps { set; get; }
        public virtual DbSet<ReelShortTemp> ReelShortTemps { set; get; }
        public virtual DbSet<ReelMoveLog> ReelAllocationLogs { set; get; }
        public virtual DbSet<ReelMoveMethod> ReelAllocationMethods { set; get; }
        public virtual DbSet<WorkBill> WorkBills { set; get; }

        //public virtual DbSet<ReadySlot> ReadySlots { set; get; }

        public virtual DbSet<ReelSupplyTemp> ReelSupplyTemps { set; get; }

        public virtual DbSet<MPNStorageAreaMap> MPNStorageAreaMaps { set; get; }
        public virtual DbSet<StorageArea> StorageAreas { set; get; }

        public virtual DbSet<ReadyMBillDetailed> ReadyMBillDetaileds { set; get; }
        public virtual DbSet<WorkBillDetailed> WorkBillDetaileds { set; get; }

        public virtual DbSet<ReadyMBill> ReadyMBills { set; get; }

        public virtual DbSet<ReadyMBillWorkBillMap> ReadyMBillWorkBillMaps { set; get; }

        public virtual DbSet<ReceivedReelBill> ReceivedReelBills { set; get; }

        public virtual DbSet<RMMStorageMap> RMMStorageMaps { set; get; }

        public virtual DbSet<PrintReel> PrintReels { set; get; }


        public WMSCloudDbContext(DbContextOptions<WMSCloudDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BarCodeAnalysis>(b => b.ToTable("WMSBarCodeAnalysis").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<BOM>(b => b.ToTable("WMSBOM").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<Customer>(b => b.ToTable("WMSCustomer").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<Line>(b => b.ToTable("WMSLine").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<MPN>(b => b.ToTable("WMSMPN").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<StorageLocation>(b => { b.ToTable("WMSStorageLocation").HasIndex(r => r.ReelId); b.Property(t => t.Id).HasMaxLength(30); });
            modelBuilder.Entity<StorageLocationType>(b => b.ToTable("WMSStorageLocationType").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<Slot>(b => b.ToTable("WMSSlot"));  // int
            modelBuilder.Entity<Storage>(b => b.ToTable("WMSStorage").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<Reel>(b => { b.ToTable("WMSReel").HasIndex(r => r.StorageLocationId); b.Property(t => t.Id).HasMaxLength(60); });
            modelBuilder.Entity<ReelSendTemp>(b => b.ToTable("WMSReelSendTemp").Property(t => t.Id).HasMaxLength(60));
            modelBuilder.Entity<ReelShortTemp>(b => b.ToTable("WMSReelShortTemp").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<ReelSupplyTemp>(b => b.ToTable("WMSReelSupplyTemp").Property(t => t.Id).HasMaxLength(60));
            //modelBuilder.Entity<ReadySlot>(b => b.ToTable("WMSReadySlot").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<StorageArea>(b => b.ToTable("WMSStorageArea").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<PrintReel>(b => b.ToTable("WMSPrintReel").Property(t => t.Id).HasMaxLength(30));


            modelBuilder.Entity<ReelMoveLog>(b => b.ToTable("WMSReelMoveMethodLog").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<ReelMoveMethod>(b => b.ToTable("WMSReelMoveMethod").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<WorkBill>(b => b.ToTable("WMSWorkBill").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<ReadyMBill>(b => b.ToTable("WMSReadyMBill").Property(t => t.Id).HasMaxLength(30));
            modelBuilder.Entity<UPH>(b => b.ToTable("WMSUPH").Property(t => t.Id).HasMaxLength(36));

            modelBuilder.Entity<ReadyMBillDetailed>(b => b.ToTable("WMSReadyMBillDetailed").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<WorkBillDetailed>(b => b.ToTable("WMSWorkBillDetailed").Property(t => t.Id).HasMaxLength(36));
            modelBuilder.Entity<ReceivedReelBill>(b => b.ToTable("WMSReceivedReelBill").Property(t => t.Id).HasMaxLength(36));


            modelBuilder.Entity<ReadyMBillWorkBillMap>(b =>
            {
                b.ToTable("WMSReadyMBillWorkBillMap");

                b.Property(t => t.Id).HasMaxLength(36);

                b.HasOne(pt => pt.WorkBill).WithMany(t => t.ReadyMBills).HasForeignKey(pt => pt.WorkBillId);

                b.HasOne(pt => pt.ReadyMBill).WithMany(t => t.WorkBills).HasForeignKey(pt => pt.ReadyMBillId);
            }

            );

            modelBuilder.Entity<RMMStorageMap>(b =>
            {
                b.ToTable("WMSRMMStorageMap");

                b.Property(t => t.Id).HasMaxLength(36);

                b.HasOne(pt => pt.ReelMoveMethod).WithMany(t => t.OutStorages).HasForeignKey(pt => pt.ReelMoveMethodId);
            }

           );

            modelBuilder.Entity<MPNStorageAreaMap>(b =>
            {
                b.ToTable("WMSMPNStorageAreaMap");

                b.Property(t => t.Id).HasMaxLength(36);

                b.HasOne(pt => pt.MPN).WithMany(t => t.StorageAreas).HasForeignKey(pt => pt.MPNId);

                b.HasOne(pt => pt.StorageArea).WithMany(t => t.MPNs).HasForeignKey(pt => pt.StorageAreaId);
            });


            modelBuilder.Entity<Role>(b => b.HasOne(pt => pt.Org).WithMany(t => t.Roles).HasForeignKey(pt => pt.OrgId));



            // modelBuilder.Entity<TestEntitie>(b => b.ToTable("MesTestEntitie"));

        }
    }
}
