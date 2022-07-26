﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations.core
{
    [DbContext(typeof(DataContext))]
    [Migration("20210823085427_AddPostgressExtensionProperty_citext")]
    partial class AddPostgressExtensionProperty_citext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("citext")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Entities.ListItem", b =>
                {
                    b.Property<int>("ListItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsSystemConfig")
                        .HasColumnType("boolean");

                    b.Property<int>("ListItemCategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("ListItemName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ListItemSystemName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("ListItemId");

                    b.HasIndex("ListItemSystemName")
                        .IsUnique();

                    b.HasIndex("ListItemCategoryId", "ListItemName")
                        .IsUnique();

                    b.ToTable("ListItem");
                });

            modelBuilder.Entity("Entities.ListItemCategory", b =>
                {
                    b.Property<int>("ListItemCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsSystemConfig")
                        .HasColumnType("boolean");

                    b.Property<string>("ListItemCategoryName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ListItemCategorySystemName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("ListItemCategoryId");

                    b.HasIndex("ListItemCategorySystemName")
                        .IsUnique();

                    b.ToTable("ListItemCategory");
                });

            modelBuilder.Entity("Entities.Person", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("PersonId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Entities.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<string>("ProjectDescription")
                        .HasColumnType("text");

                    b.Property<int>("ProjectMarketListItemId")
                        .HasColumnType("integer");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("citext");

                    b.Property<string>("ProjectSlug")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("ProjectId");

                    b.HasIndex("ProjectMarketListItemId");

                    b.HasIndex("ProjectName")
                        .IsUnique();

                    b.HasIndex("ProjectSlug")
                        .IsUnique();

                    b.ToTable("Project");
                });

            modelBuilder.Entity("Entities.ProjectMember", b =>
                {
                    b.Property<int>("ProjectMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<int>("PersonId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectRoleListItemId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("ProjectMemberId");

                    b.HasIndex("PersonId");

                    b.HasIndex("ProjectRoleListItemId");

                    b.HasIndex("ProjectId", "PersonId")
                        .IsUnique();

                    b.ToTable("ProjectMember");
                });

            modelBuilder.Entity("Entities.ListItem", b =>
                {
                    b.HasOne("Entities.ListItemCategory", "ListItemCategory")
                        .WithMany()
                        .HasForeignKey("ListItemCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ListItemCategory");
                });

            modelBuilder.Entity("Entities.Project", b =>
                {
                    b.HasOne("Entities.ListItem", "ProjectMarketListItem")
                        .WithMany()
                        .HasForeignKey("ProjectMarketListItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectMarketListItem");
                });

            modelBuilder.Entity("Entities.ProjectMember", b =>
                {
                    b.HasOne("Entities.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.ListItem", "ProjectRoleListItem")
                        .WithMany()
                        .HasForeignKey("ProjectRoleListItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");

                    b.Navigation("Project");

                    b.Navigation("ProjectRoleListItem");
                });
#pragma warning restore 612, 618
        }
    }
}