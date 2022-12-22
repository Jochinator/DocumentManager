﻿// <auto-generated />
using System;
using DocumentManagerPersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DocumentManagerPersistence.Migrations
{
    [DbContext(typeof(DocumentContext))]
    partial class DocumentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.2");

            modelBuilder.Entity("DocumentManager.DocumentMetadataDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Checked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SenderName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Metadatas");
                });

            modelBuilder.Entity("DocumentManager.TagDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DocumentMetadataDaoId")
                        .HasColumnType("TEXT");

                    b.Property<string>("value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DocumentMetadataDaoId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DocumentManager.TagDao", b =>
                {
                    b.HasOne("DocumentManager.DocumentMetadataDao", null)
                        .WithMany("Tags")
                        .HasForeignKey("DocumentMetadataDaoId");
                });

            modelBuilder.Entity("DocumentManager.DocumentMetadataDao", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
