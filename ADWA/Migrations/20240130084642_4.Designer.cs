﻿// <auto-generated />
using ADWA.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ADWA.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20240130084642_4")]
    partial class _4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ADWA.Models.ApplicationUser", b =>
                {
                    b.Property<string>("SamAccountName")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "SamAccountName");

                    b.HasKey("SamAccountName");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
