﻿using GraduationProject.models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(I => I.Courses)
                .HasForeignKey(C => C.Instructor_Id)
                .OnDelete(DeleteBehavior.Cascade);
           

            modelBuilder.Entity<CourseTag>()
                .HasKey(CT => new { CT.CourseId, CT.TagId });

            modelBuilder.Entity<CourseTag>()
                 .HasOne(ct => ct.Course)
                 .WithMany(c => c.CourseTags)
                 .HasForeignKey(ct => ct.CourseId);
            modelBuilder.Entity<CourseTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CourseTags)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Section>()
                .HasOne(x => x.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Lesson>()
                .HasOne(l =>l.Section)
                .WithMany(s =>s.Lessons)
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<LessonTag>()
            //    .HasKey(ct => new { ct.LessonId, ct.TagId });
            //modelBuilder.Entity<LessonTag>()
            //     .HasOne(ct => ct.Lesson)
            //     .WithMany(c => c.LessonTags)
            //     .HasForeignKey(ct => ct.LessonId)
            //     .OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<LessonTag>()
            //    .HasOne(ct => ct.Tag)
            //    .WithMany(t => t.lessonTags)
            //    .HasForeignKey(ct => ct.TagId)
            //    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Lesson)
                .WithMany(l => l.Questions)
                .HasForeignKey(q => q.LessonId);
            modelBuilder.Entity<Answer>()
                .HasOne(a =>a.Question)
                .WithMany(q =>q.Answers)
                .HasForeignKey(a =>a.QuestionId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Rating )
                .HasForeignKey(r => r.CourseId);
            modelBuilder.Entity<Rating>()
                .HasOne(r =>r.student)
                .WithMany()
                .HasForeignKey(r =>r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
           modelBuilder.Entity<VideoProgress>()
          .HasOne(vp => vp.Lesson)
          .WithMany()
          .HasForeignKey(vp => vp.LessonId)
          .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VideoProgress>()
          .HasOne(vp => vp.Student)
          .WithMany()
          .HasForeignKey(vp => vp.StudentId)
          .OnDelete(DeleteBehavior.NoAction); 

          base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> users { get; set; }
        public DbSet<Course> courses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<Lesson> Lesson { get; set; }
        //public DbSet<LessonTag> LessonTag { get; set; }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public DbSet<Rating> Rating { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<VideoProgress> VideoProgress { get; set; }
    }
}
