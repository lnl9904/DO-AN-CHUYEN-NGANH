﻿using BlogWebsite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<ApplicationUser>? applicationUsers { get; set; }
		public DbSet<Post>? posts { get; set; }
		public DbSet<ForumPost>? forumPosts { get; set; }
		public DbSet<Page>? pages { get; set; }
		public DbSet<Tag>? tags { get; set; }
		public DbSet<Topic>? topics { get; set; }
		public DbSet<Comment>? comments { get; set; }
		public DbSet<Setting>? settings { get; set; }
		public DbSet<Reaction>? reactions { get; set; }
		public DbSet<RegistrationPeriods>? registrationPeriods { get; set; }
		public DbSet<WritingPhases>? writingPhases { get; set; }
		public DbSet<UserCapacities>? userCapacities { get; set; }
		public DbSet<Assignment>? assignments { get; set; }
	}
}
