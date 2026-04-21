import { useEffect, useState } from 'react';
import CreatePost from '../components/specific/CreatePost';
import PostCard from '../components/specific/PostCard';
import PostSkeleton from '../components/common/PostSkeleton';
import { usePosts } from '../hooks/usePosts';
import { getTrendingHashtags } from '../services/hashtags';
import type { HashtagTrendingResult } from '../types';

const Home = () => {
  const { posts, getPosts, createPost, loading, error } = usePosts();
  const [trending, setTrending] = useState<HashtagTrendingResult[]>([]);
  const [trendingLoading, setTrendingLoading] = useState(true);
  const [trendingError, setTrendingError] = useState<string | null>(null);

  const loadTrending = async () => {
    setTrendingLoading(true);
    setTrendingError(null);
    try {
      const data = await getTrendingHashtags(1, 6);
      setTrending(data);
    } catch (err: any) {
      setTrendingError(err?.message || 'Unable to load trending hashtags.');
    } finally {
      setTrendingLoading(false);
    }
  };

  useEffect(() => {
    getPosts().catch(() => undefined);
    loadTrending().catch(() => undefined);
  }, [getPosts]);

  return (
    <div className="max-w-2xl mx-auto">
      <CreatePost onCreate={createPost} />

      <div className="mt-4 rounded-2xl border border-white/60 bg-white/70 px-4 py-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] backdrop-blur-xl dark:border-slate-800/60 dark:bg-slate-900/60">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-sm font-semibold text-slate-900 dark:text-slate-100">Trending hashtags</h2>
            <p className="text-xs text-slate-500 dark:text-slate-400">Popular topics right now.</p>
          </div>
          <button
            onClick={() => loadTrending()}
            className="text-xs font-semibold text-slate-500 hover:text-slate-900 dark:text-slate-400 dark:hover:text-slate-100"
            type="button"
          >
            Refresh
          </button>
        </div>

        {trendingLoading && (
          <div className="mt-3 text-sm text-slate-500 dark:text-slate-400">Loading trending tags...</div>
        )}
        {trendingError && (
          <div className="mt-3 rounded-xl border border-rose-200/70 bg-rose-50/70 px-3 py-2 text-xs text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
            {trendingError}
          </div>
        )}

        {!trendingLoading && !trendingError && trending.length === 0 && (
          <div className="mt-3 text-sm text-slate-500 dark:text-slate-400">No hashtags trending yet.</div>
        )}

        {!trendingLoading && trending.length > 0 && (
          <div className="mt-3 flex flex-wrap gap-2">
            {trending.map((tag) => (
              <div
                key={tag.hashtagId}
                className="flex items-center gap-2 rounded-full border border-cyan-100 bg-white/80 px-3 py-1 text-xs font-semibold text-cyan-700 dark:border-cyan-500/30 dark:bg-slate-900/70 dark:text-cyan-200"
              >
                <span>{tag.tag}</span>
                <span className="rounded-full bg-cyan-100/70 px-2 py-0.5 text-[10px] text-cyan-700 dark:bg-cyan-500/10 dark:text-cyan-200">
                  {tag.usageCount}
                </span>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="space-y-4 mt-4">
        {error && (
          <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
            {error}
          </div>
        )}
        {loading ? (
          <>
            <PostSkeleton />
            <PostSkeleton />
          </>
        ) : (
          posts.map(p => <PostCard key={p.postId} post={p} />)
        )}
      </div>
    </div>
  );
};

export default Home;

