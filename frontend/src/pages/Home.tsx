import { useEffect } from 'react';
import CreatePost from '../components/specific/CreatePost';
import PostCard from '../components/specific/PostCard';
import PostSkeleton from '../components/common/PostSkeleton';
import { usePosts } from '../hooks/usePosts';

const Home = () => {
  const { posts, getPosts, createPost, loading, error } = usePosts();

  useEffect(() => {
    getPosts().catch(() => undefined);
  }, [getPosts]);

  return (
    <div className="max-w-2xl mx-auto">
      <CreatePost onCreate={createPost} />

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

