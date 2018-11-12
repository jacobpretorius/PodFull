using System.Collections.Generic;
using System.Linq;
using PodFull.Web.Business.Helpers;
using PodFull.Web.Models.Data;

namespace PodFull.Web.Business.Cache
{
	public static class MemoryCache
	{
		private static List<Episode> _memoryCache = new List<Episode>();

		static MemoryCache()
		{
			//fill the cache with all posts, ordered by storage helper
			_memoryCache = StorageHelper.ReadAllPosts();
		}

		/// <summary>
		/// Get a Episode by slug from memory and then from disk.
		/// </summary>
		/// <param name="Id">the Id search</param>
		/// <returns>Episode if found in memory or disk, else returns null</returns>
		public static Episode GetPost(int id)
		{
			if (_memoryCache?.Count(w => w?.Id == id) > 0)
			{
				//cache hit
				return _memoryCache?.FirstOrDefault(w => w.Id == id);
			}

			//cache miss
			var missed = StorageHelper.ReadPost(id);
			if (missed != null)
			{
				_memoryCache?.Add(missed);

				//sort now that it is added
				_memoryCache.Sort((p1, p2) => p2.Id.CompareTo(p1.Id));

				return missed;
			}

			return null;
		}

		/// <summary>
		/// Get the 10 newest posts
		/// </summary>
		/// <returns>10 Posts in reverse order</returns>
		public static IEnumerable<Episode> GetTenPosts(int pager = 0)
		{
			return _memoryCache?.Skip(pager * 10)?.Take(10);
		}

		/// <summary>
		/// Add or Updates the memory Cache with a post
		/// </summary>
		public static void AddPost(Episode post)
		{
			//check if we have it in the cache
			if (_memoryCache.Count(w => w.Id == post.Id) > 0)
			{
				//we do have it, remove old one first
				_memoryCache.RemoveAll(s => s.Id == post.Id);
			}

			//now add it
			_memoryCache.Add(post);

			//and sort for good measure
			_memoryCache.Sort((p1, p2) => p2.Id.CompareTo(p1.Id));
		}

		/// <summary>
		/// remove a post from the Cache
		/// </summary>
		public static void RemovePost(int id)
		{
			//check if we have it in the cache
			if (_memoryCache.Count(w => w.Id == id) > 0)
			{
				//we do have it, remove
				_memoryCache.RemoveAll(s => s.Id == id);
			}
		}
	}
}