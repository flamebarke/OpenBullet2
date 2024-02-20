﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenBullet2.Core;

#nullable disable

namespace OpenBullet2.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240220175839_ProxyGroupDeleteRelationships")]
    partial class ProxyGroupDeleteRelationships
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("OpenBullet2.Core.Entities.GuestEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AccessExpiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("AllowedAddresses")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.HitEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CapturedData")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConfigCategory")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConfigId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConfigName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Proxy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<int>("WordlistId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WordlistName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Hits");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.JobEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("JobOptions")
                        .HasColumnType("TEXT");

                    b.Property<int>("JobType")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.ProxyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<int?>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Host")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastChecked")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<int>("Ping")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Proxies");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.ProxyGroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("ProxyGroups");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.RecordEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Checkpoint")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConfigId")
                        .HasColumnType("TEXT");

                    b.Property<int>("WordlistId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.WordlistEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Purpose")
                        .HasColumnType("TEXT");

                    b.Property<int>("Total")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Wordlists");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.JobEntity", b =>
                {
                    b.HasOne("OpenBullet2.Core.Entities.GuestEntity", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.ProxyEntity", b =>
                {
                    b.HasOne("OpenBullet2.Core.Entities.ProxyGroupEntity", "Group")
                        .WithMany("Proxies")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Group");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.ProxyGroupEntity", b =>
                {
                    b.HasOne("OpenBullet2.Core.Entities.GuestEntity", "Owner")
                        .WithMany("ProxyGroups")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.WordlistEntity", b =>
                {
                    b.HasOne("OpenBullet2.Core.Entities.GuestEntity", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.GuestEntity", b =>
                {
                    b.Navigation("ProxyGroups");
                });

            modelBuilder.Entity("OpenBullet2.Core.Entities.ProxyGroupEntity", b =>
                {
                    b.Navigation("Proxies");
                });
#pragma warning restore 612, 618
        }
    }
}
