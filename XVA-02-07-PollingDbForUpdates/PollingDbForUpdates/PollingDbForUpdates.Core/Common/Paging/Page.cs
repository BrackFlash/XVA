using System;
using System.Collections.Generic;
using PollingDbForUpdates.Core.Interfaces.Paging;
	using PollingDbForUpdates.Core.Model;
	
namespace PollingDbForUpdates.Core.Common.Paging
{
    [Serializable()]
    public class Page<T> : IPage<T> where T : PersistentEntity
    {
        public int CurrentPage { get; set; }
        public int PagesCount { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IEnumerable<T> Entities { get; set; }

        public Page(IEnumerable<T> entities, int count, int pageSize, int currentPage)
        {
            this.Entities = entities;
            this.Count = count;
            this.CurrentPage = currentPage;
            this.PageSize = pageSize;
            this.PagesCount = count <= pageSize ? 1 : DivideRoundingUp(count, pageSize);
        }

        private static int DivideRoundingUp(int x, int y)
        {
            int remainder;
            var quotient = Math.DivRem(x, y, out remainder);
            return remainder == 0 ? quotient : quotient + 1;
        }

        public Page()
        {
        }
    } 
}
