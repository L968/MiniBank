﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniBank.Api.Infrastructure;

#nullable disable

namespace MiniBank.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250202070528_AddingTransactionMessageColumn")]
    partial class AddingTransactionMessageColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("MiniBank.Api.Domain.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PayeeId")
                        .HasColumnType("int");

                    b.Property<int>("PayerId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("Value")
                        .HasPrecision(65, 2)
                        .HasColumnType("decimal(65,2)");

                    b.HasKey("Id");

                    b.HasIndex("PayeeId");

                    b.HasIndex("PayerId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("MiniBank.Api.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Balance")
                        .HasPrecision(65, 2)
                        .HasColumnType("decimal(65,2)");

                    b.Property<string>("CpfCnpj")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MiniBank.Api.Domain.Transaction", b =>
                {
                    b.HasOne("MiniBank.Api.Domain.User", "Payee")
                        .WithMany()
                        .HasForeignKey("PayeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiniBank.Api.Domain.User", "Payer")
                        .WithMany()
                        .HasForeignKey("PayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Payee");

                    b.Navigation("Payer");
                });
#pragma warning restore 612, 618
        }
    }
}
