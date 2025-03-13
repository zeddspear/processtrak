﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using processtrak_backend.Api.data;

#nullable disable

namespace processtrak_backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250313180135_Just_A_DB_INIT")]
    partial class Just_A_DB_INIT
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AlgorithmSchedule", b =>
                {
                    b.Property<Guid>("algorithmsid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("scheduleRunsid")
                        .HasColumnType("uuid");

                    b.HasKey("algorithmsid", "scheduleRunsid");

                    b.HasIndex("scheduleRunsid");

                    b.ToTable("AlgorithmSchedule");
                });

            modelBuilder.Entity("processtrak_backend.Models.Algorithm", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("createdAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime?>("deletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("description")
                        .HasColumnType("text");

                    b.Property<string>("displayName")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("requiresTimeQuantum")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("updatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("id");

                    b.ToTable("Algorithms");
                });

            modelBuilder.Entity("processtrak_backend.Models.OtpCode", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("createdAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime?>("deletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("expiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("updatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("id");

                    b.ToTable("OtpCodes");
                });

            modelBuilder.Entity("processtrak_backend.Models.Process", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid?>("Scheduleid")
                        .HasColumnType("uuid");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<int?>("arrivalTime")
                        .HasColumnType("integer");

                    b.Property<int?>("burstTime")
                        .HasColumnType("integer");

                    b.Property<int?>("completionTime")
                        .HasColumnType("integer");

                    b.Property<DateTime>("createdAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime?>("deletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("isCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("priority")
                        .HasColumnType("integer");

                    b.Property<string>("processId")
                        .HasColumnType("text");

                    b.Property<int?>("remainingTime")
                        .HasColumnType("integer");

                    b.Property<int?>("responseTime")
                        .HasColumnType("integer");

                    b.Property<int?>("startTime")
                        .HasColumnType("integer");

                    b.Property<int?>("turnaroundTime")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("updatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("userId")
                        .HasColumnType("uuid");

                    b.Property<int?>("waitingTime")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("Scheduleid");

                    b.HasIndex("userId");

                    b.ToTable("Processes");
                });

            modelBuilder.Entity("processtrak_backend.Models.Schedule", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("averageTurnaroundTime")
                        .HasColumnType("integer");

                    b.Property<int>("averageWaitingTime")
                        .HasColumnType("integer");

                    b.Property<DateTime>("createdAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime?>("deletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("endTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("startTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("totalExecutionTime")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("updatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("processtrak_backend.Models.User", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("createdAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime?>("deletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("updatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("processtrak_backend.Models.UserSession", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTime>("createdAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime?>("deletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("expiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("updatedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("userId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("AlgorithmSchedule", b =>
                {
                    b.HasOne("processtrak_backend.Models.Algorithm", null)
                        .WithMany()
                        .HasForeignKey("algorithmsid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("processtrak_backend.Models.Schedule", null)
                        .WithMany()
                        .HasForeignKey("scheduleRunsid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("processtrak_backend.Models.Process", b =>
                {
                    b.HasOne("processtrak_backend.Models.Schedule", null)
                        .WithMany("processes")
                        .HasForeignKey("Scheduleid");

                    b.HasOne("processtrak_backend.Models.User", "user")
                        .WithMany("Processes")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("processtrak_backend.Models.UserSession", b =>
                {
                    b.HasOne("processtrak_backend.Models.User", "user")
                        .WithMany("UserSessions")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("processtrak_backend.Models.Schedule", b =>
                {
                    b.Navigation("processes");
                });

            modelBuilder.Entity("processtrak_backend.Models.User", b =>
                {
                    b.Navigation("Processes");

                    b.Navigation("UserSessions");
                });
#pragma warning restore 612, 618
        }
    }
}
