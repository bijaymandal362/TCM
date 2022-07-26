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
    [Migration("20210903114534_AddTableTestCase")]
    partial class AddTableTestCase
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

            modelBuilder.Entity("Entities.Menu", b =>
                {
                    b.Property<int>("MenuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<string>("MenuName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MenuSlug")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("MenuId");

                    b.HasIndex("MenuSlug")
                        .IsUnique();

                    b.ToTable("Menu");
                });

            modelBuilder.Entity("Entities.MenuPermission", b =>
                {
                    b.Property<int>("MenuPermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<int>("MenuId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("MenuPermissionId");

                    b.HasIndex("MenuId");

                    b.HasIndex("RoleId");

                    b.ToTable("MenuPermission");
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

                    b.Property<int>("UserMarketListItemId")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("UserRoleListItemId")
                        .HasColumnType("integer");

                    b.HasKey("PersonId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserMarketListItemId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.HasIndex("UserRoleListItemId");

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

                    b.Property<DateTimeOffset>("StartDate")
                        .HasColumnType("timestamp with time zone");

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

            modelBuilder.Entity("Entities.ProjectModule", b =>
                {
                    b.Property<int>("ProjectModuleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<string>("ModuleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ParentProjectModuleId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectModuleListItemId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("ProjectModuleId");

                    b.HasIndex("ProjectModuleListItemId");

                    b.HasIndex("ProjectId", "ModuleName", "ParentProjectModuleId")
                        .IsUnique();

                    b.ToTable("ProjectModule");
                });

            modelBuilder.Entity("Entities.ProjectModuleDeveloper", b =>
                {
                    b.Property<int>("ProjectModuleDeveloperId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<int>("ProjectMemberId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectModuleId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("ProjectModuleDeveloperId");

                    b.HasIndex("ProjectMemberId");

                    b.HasIndex("ProjectModuleId", "ProjectMemberId")
                        .IsUnique();

                    b.ToTable("ProjectModuleDeveloper");
                });

            modelBuilder.Entity("Entities.TestCaseDetail", b =>
                {
                    b.Property<int>("TestCaseDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ExpectedResult")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTimeOffset>("InsertDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsertPersonId")
                        .HasColumnType("integer");

                    b.Property<string>("PreCondition")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int>("ProjectModuleId")
                        .HasColumnType("integer");

                    b.Property<string>("TestCases")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UpdatePersonId")
                        .HasColumnType("integer");

                    b.HasKey("TestCaseDetailId");

                    b.HasIndex("ProjectModuleId")
                        .IsUnique();

                    b.ToTable("TestCaseDetail");
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

            modelBuilder.Entity("Entities.MenuPermission", b =>
                {
                    b.HasOne("Entities.Menu", "Menu")
                        .WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.ListItem", "RoleListItem")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Menu");

                    b.Navigation("RoleListItem");
                });

            modelBuilder.Entity("Entities.Person", b =>
                {
                    b.HasOne("Entities.ListItem", "UserMarketListItem")
                        .WithMany()
                        .HasForeignKey("UserMarketListItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.ListItem", "UserRoleListItem")
                        .WithMany()
                        .HasForeignKey("UserRoleListItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserMarketListItem");

                    b.Navigation("UserRoleListItem");
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

            modelBuilder.Entity("Entities.ProjectModule", b =>
                {
                    b.HasOne("Entities.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.ListItem", "ProjectModuleListItem")
                        .WithMany()
                        .HasForeignKey("ProjectModuleListItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("ProjectModuleListItem");
                });

            modelBuilder.Entity("Entities.ProjectModuleDeveloper", b =>
                {
                    b.HasOne("Entities.ProjectMember", "ProjectMember")
                        .WithMany()
                        .HasForeignKey("ProjectMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.ProjectModule", "ProjectModule")
                        .WithMany()
                        .HasForeignKey("ProjectModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectMember");

                    b.Navigation("ProjectModule");
                });

            modelBuilder.Entity("Entities.TestCaseDetail", b =>
                {
                    b.HasOne("Entities.ProjectModule", "ProjectModule")
                        .WithMany()
                        .HasForeignKey("ProjectModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectModule");
                });
#pragma warning restore 612, 618
        }
    }
}
