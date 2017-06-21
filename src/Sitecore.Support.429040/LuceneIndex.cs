using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Maintenance;
using System.Threading;
using Sitecore.ContentSearch.Diagnostics;

namespace Sitecore.Support.ContentSearch.LuceneProvider 
{
    public class LuceneIndex : Sitecore.ContentSearch.LuceneProvider.LuceneIndex
    {
        public LuceneIndex(string name, string folder, IIndexPropertyStore propertyStore)
            : base(name, folder, propertyStore)
        {
            // do nothing
        }

        public LuceneIndex(string name, string folder, IIndexPropertyStore propertyStore, string group)
            : base(name, folder, propertyStore, group)
        {
            // do nothing
        }

        protected override void PerformRefresh(IIndexable indexableStartingPoint, IndexingOptions indexingOptions, CancellationToken cancellationToken)
        {
            base.VerifyNotDisposed();
            if (base.ShouldStartIndexing(indexingOptions))
            {
                lock (this.indexUpdateLock)
                {
                    if (base.Crawlers.Any<IProviderCrawler>(c => c.HasItemsToIndex()))
                    {
                        using (IProviderUpdateContext context = this.CreateUpdateContext())
                        {
                            foreach (IProviderCrawler crawler in base.Crawlers)
                            {
                                crawler.RefreshFromRoot(context, indexableStartingPoint, indexingOptions, cancellationToken);
                            }
                            context.Commit();
                            CrawlingLog.Log.Debug(context.GetType().FullName + ".Optimize() was not called.");
                        }
                    }
                }
            }
        }
    }
}