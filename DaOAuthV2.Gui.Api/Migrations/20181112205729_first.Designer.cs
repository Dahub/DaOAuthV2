﻿// <auto-generated />
using System;
using DaOAuthV2.Dal.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DaOAuthV2.Gui.Api.Migrations
{
    [DbContext(typeof(DaOAuthContext))]
    [Migration("20181112205729_first")]
    partial class first
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("auth")
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DaOAuthV2.Domain.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("ClientSecret")
                        .HasColumnName("ClientSecret")
                        .HasColumnType("varbinary(50)")
                        .HasMaxLength(50);

                    b.Property<int>("ClientTypeId")
                        .HasColumnName("FK_ClientType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnName("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValid")
                        .HasColumnName("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnName("PublicId")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("ClientTypeId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.ClientReturnUrl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("FK_Client")
                        .HasColumnType("int");

                    b.Property<string>("ReturnUrl")
                        .IsRequired()
                        .HasColumnName("ReturnUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientReturnUrls");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.ClientScope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("FK_Client")
                        .HasColumnType("int");

                    b.Property<int>("ScopeId")
                        .HasColumnName("FK_Scope")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ScopeId");

                    b.ToTable("ClientsScopes");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.ClientType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Wording")
                        .IsRequired()
                        .HasColumnName("Wording")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("ClientsTypes");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.Code", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("FK_Client")
                        .HasColumnType("int");

                    b.Property<string>("CodeValue")
                        .IsRequired()
                        .HasColumnName("CodeValue")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<long>("ExpirationTimeStamp")
                        .HasColumnName("ExpirationTimeStamp")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsValid")
                        .HasColumnName("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Scope")
                        .HasColumnName("Scope")
                        .HasColumnType("nvarchar(max)")
                        .HasMaxLength(2147483647);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnName("UserName")
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.Property<Guid>("UserPublicId")
                        .HasColumnName("UserPublicId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Codes");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.RessourceServer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValid")
                        .HasColumnName("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnName("Login")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<byte[]>("ServerSecret")
                        .HasColumnName("ServerSecret")
                        .HasColumnType("varbinary(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("RessourceServers");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.Scope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("NiceWording")
                        .HasColumnName("NiceWording")
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.Property<string>("Wording")
                        .HasColumnName("Wording")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Scopes");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnName("BirthDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("CreationDate")
                        .IsRequired()
                        .HasColumnName("CreationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("EMail")
                        .IsRequired()
                        .HasColumnName("Email")
                        .HasColumnType("nvarchar(320)");

                    b.Property<string>("FullName")
                        .HasColumnName("FullName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("IsValid")
                        .HasColumnName("IsValid")
                        .HasColumnType("bit");

                    b.Property<byte[]>("Password")
                        .HasColumnName("Password")
                        .HasColumnType("varbinary(50)")
                        .HasMaxLength(50);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnName("UserName")
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.UserClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("FK_Client")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnName("CreationDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsValid")
                        .HasColumnName("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("RefreshToken")
                        .HasColumnName("RefreshToken")
                        .HasColumnType("nvarchar(max)")
                        .HasMaxLength(2147483647);

                    b.Property<int>("UserId")
                        .HasColumnName("FK_User")
                        .HasColumnType("int");

                    b.Property<Guid>("UserPublicId")
                        .HasColumnName("UserPublicId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersClients");
                });

            modelBuilder.Entity("DaOAuthV2.Domain.Client", b =>
                {
                    b.HasOne("DaOAuthV2.Domain.ClientType", "ClientType")
                        .WithMany("Clients")
                        .HasForeignKey("ClientTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DaOAuthV2.Domain.ClientReturnUrl", b =>
                {
                    b.HasOne("DaOAuthV2.Domain.Client", "Client")
                        .WithMany("ClientReturnUrls")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DaOAuthV2.Domain.ClientScope", b =>
                {
                    b.HasOne("DaOAuthV2.Domain.Client", "Client")
                        .WithMany("ClientsScopes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DaOAuthV2.Domain.Scope", "Scope")
                        .WithMany("ClientsScopes")
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DaOAuthV2.Domain.Code", b =>
                {
                    b.HasOne("DaOAuthV2.Domain.Client", "Client")
                        .WithMany("Codes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DaOAuthV2.Domain.UserClient", b =>
                {
                    b.HasOne("DaOAuthV2.Domain.Client", "Client")
                        .WithMany("UsersClients")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DaOAuthV2.Domain.User", "User")
                        .WithMany("UsersClients")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
