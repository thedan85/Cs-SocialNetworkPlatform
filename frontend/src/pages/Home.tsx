import { useEffect, useState } from 'react';
import CreatePost from '../components/specific/CreatePost';
import PostCard from '../components/specific/PostCard';
import PostSkeleton from '../components/common/PostSkeleton';
import { Post } from '../types';

const Home = () => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Simulate fetching posts from API
    setTimeout(() => {
      setPosts([
        {
          id: '1',
          content: 'Hello world! This is my first post on this awesome social network 🌍',
          createdAt: new Date().toISOString(),
          likesCount: 5,
          imageUrl: 'https://picsum.photos/500?random=1',
          author: {
            id: '1',
            username: 'john_doe',
            email: 'john@example.com'
          }
        },
        {
          id: '2',
          content: 'Just finished an amazing project with React and TypeScript! 💻',
          createdAt: new Date(Date.now() - 3600000).toISOString(),
          likesCount: 12,
          imageUrl: 'https://picsum.photos/500?random=2',
          author: {
            id: '2',
            username: 'jane_smith',
            email: 'jane@example.com'
          }
        }
      ]);
      setLoading(false);
    }, 1500);
  }, []);

  return (
    <div className="max-w-2xl mx-auto">
      <CreatePost />

      <div className="space-y-4 mt-4">
        {loading ? (
          <>
            <PostSkeleton />
            <PostSkeleton />
          </>
        ) : (
          posts.map(p => <PostCard key={p.id} post={p} />)
        )}
      </div>
    </div>
  );
};

export default Home;

