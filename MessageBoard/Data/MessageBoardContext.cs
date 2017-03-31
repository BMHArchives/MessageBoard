using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MessageBoard.Data
{
    public class MessageBoardContext: DbContext
    {
        public MessageBoardContext():base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false; //avoids cicular refernence
            this.Configuration.ProxyCreationEnabled = false; // Don't use, because we're trying to handle new field changes our way.

            /* Use for code first migrations - takes an instance of an object that will handle the ininizations (for migration new changes to the db.. */
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MessageBoardContext,MessageBoardMigrationsConfiguration>());
           
      
        }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Reply> Replies { get; set; }

    }
}