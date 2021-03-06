﻿// <auto-generated />
using CountyRP.WebAPI.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CountyRP.WebAPI.Migrations.Faction
{
    [DbContext(typeof(FactionContext))]
    partial class FactionContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CountyRP.DAO.Faction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("_Ranks")
                        .HasColumnName("Ranks")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Factions");
                });

            modelBuilder.Entity("CountyRP.DAO.LockerRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Dimension")
                        .HasColumnType("bigint");

                    b.Property<string>("FactionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeMarker")
                        .HasColumnType("int");

                    b.Property<string>("_ColorMarker")
                        .HasColumnName("ColorMarker")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("_Position")
                        .HasColumnName("Position")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LockerRooms");
                });
#pragma warning restore 612, 618
        }
    }
}
