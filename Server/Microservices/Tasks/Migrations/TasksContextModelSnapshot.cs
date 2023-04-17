﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tasks.Repositories;

#nullable disable

namespace Tasks.Migrations
{
    [DbContext(typeof(TasksContext))]
    partial class TasksContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TaskTagTask_", b =>
                {
                    b.Property<string>("TagsId")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("TasksId")
                        .HasColumnType("char(36)");

                    b.HasKey("TagsId", "TasksId");

                    b.HasIndex("TasksId");

                    b.ToTable("TaskTagTask_");
                });

            modelBuilder.Entity("Types.Entities.TaskImplementation", b =>
                {
                    b.Property<Guid>("TaskId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Language")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Version")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CompletedSolution")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Details")
                        .HasColumnType("longtext");

                    b.Property<string>("ExampleTests")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("InitialSolution")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PreloadedCode")
                        .HasColumnType("longtext");

                    b.Property<Guid?>("Task_Id")
                        .HasColumnType("char(36)");

                    b.Property<string>("Tests")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("TaskId", "Language", "Version");

                    b.HasIndex("Task_Id");

                    b.ToTable("Implementations");
                });

            modelBuilder.Entity("Types.Entities.TaskTag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Types.Entities.Task_", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Beta")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasMaxLength(4096)
                        .HasColumnType("varchar(4096)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Likes")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Visibility")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Types.Entities.UserIdRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("UserIdRecords");
                });

            modelBuilder.Entity("TaskTagTask_", b =>
                {
                    b.HasOne("Types.Entities.TaskTag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Types.Entities.Task_", null)
                        .WithMany()
                        .HasForeignKey("TasksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Types.Entities.TaskImplementation", b =>
                {
                    b.HasOne("Types.Entities.Task_", null)
                        .WithMany("Implementations")
                        .HasForeignKey("Task_Id");
                });

            modelBuilder.Entity("Types.Entities.Task_", b =>
                {
                    b.Navigation("Implementations");
                });
#pragma warning restore 612, 618
        }
    }
}
