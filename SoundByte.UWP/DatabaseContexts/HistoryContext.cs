/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using Microsoft.EntityFrameworkCore;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.UWP.DatabaseContexts
{
    public class HistoryContext : DbContext
    {
        public DbSet<BaseTrack> Tracks { get; set; }
        public DbSet<BaseUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=sb.core.history.db"); 
        }    
    }
}
