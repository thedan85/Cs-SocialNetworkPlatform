import { Post } from '../../types';
import { Heart, MessageCircle, Share2 } from 'lucide-react';
import { useState } from 'react';

interface PostCardProps {
  post: Post;
}

const PostCard: React.FC<PostCardProps> = ({ post }) => {
  const [isLiked, setIsLiked] = useState(false);
  const [likesCount, setLikesCount] = useState(post.likesCount);

  const handleLike = () => {
    setIsLiked(!isLiked);
    setLikesCount(isLiked ? likesCount - 1 : likesCount + 1);
  };

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-4 hover:shadow-md transition-shadow">
      <div className="flex items-center gap-3 mb-3">
        <img
          src={post.author.avatarUrl || `https://ui-avatars.com/api/?name=${post.author.username}`}
          className="w-10 h-10 rounded-full object-cover"
          alt={post.author.username}
        />
        <div>
          <p className="font-semibold text-gray-800">{post.author.username}</p>
          <span className="text-xs text-gray-400">
            {new Date(post.createdAt).toLocaleString()}
          </span>
        </div>
      </div>

      <p className="text-gray-700 mb-3">{post.content}</p>

      {post.imageUrl && (
        <img
          src={post.imageUrl}
          className="w-full rounded-lg mb-3 max-h-96 object-cover"
          alt="Post image"
        />
      )}

      <div className="flex items-center gap-6 text-gray-500 text-sm border-t pt-3">
        <button 
          onClick={handleLike}
          className={`flex items-center gap-1 transition-colors ${
            isLiked ? 'text-red-500 font-semibold' : 'hover:text-red-500'
          }`}
        >
          <Heart className={`w-4 h-4 ${isLiked ? 'fill-current' : ''}`} />
          {likesCount}
        </button>

        <button className="flex items-center gap-1 hover:text-blue-500 transition-colors">
          <MessageCircle className="w-4 h-4" />
          Comment
        </button>

        <button className="flex items-center gap-1 hover:text-green-500 transition-colors">
          <Share2 className="w-4 h-4" />
          Share
        </button>
      </div>
    </div>
  );
};

export default PostCard;

