import { useState } from "react";
import {
  X,
  Heart,
  MessageCircle,
  Send,
  Bookmark,
  MoreHorizontal,
  Smile,
} from "lucide-react";
import { motion, AnimatePresence } from "motion/react";

interface Comment {
  id: string;
  user: {
    name: string;
    avatar: string;
  };
  text: string;
  timestamp: string;
  likes: number;
}

interface Post {
  id: string;
  user: {
    name: string;
    avatar: string;
  };
  image: string;
  caption: string;
  likes: number;
  timestamp: string;
  comments: Comment[];
}

interface PostDetailModalProps {
  post: Post;
  isOpen: boolean;
  onClose: () => void;
}

export function PostDetailModal({
  post,
  isOpen,
  onClose,
}: PostDetailModalProps) {
  const [newComment, setNewComment] = useState("");
  const [isLiked, setIsLiked] = useState(false);
  const [isSaved, setIsSaved] = useState(false);

  const handleSubmitComment = (e: React.FormEvent) => {
    e.preventDefault();
    if (newComment.trim()) {
      console.log("New comment:", newComment);
      setNewComment("");
    }
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Backdrop */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.2 }}
            className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50"
            onClick={onClose}
          />

          {/* Modal */}
          <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 20 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 20 }}
              transition={{ duration: 0.3, ease: [0.19, 1, 0.22, 1] }}
              className="relative bg-white rounded-2xl overflow-hidden w-full max-w-6xl max-h-[90vh] flex shadow-2xl"
              onClick={(e) => e.stopPropagation()}
              style={{ fontFamily: "'DM Sans', sans-serif" }}
            >
              {/* Close Button */}
              <button
                onClick={onClose}
                className="absolute top-4 right-4 z-10 p-2 rounded-full bg-white/90 backdrop-blur-sm hover:bg-white transition-all duration-200 shadow-lg hover:shadow-xl hover:scale-110"
              >
                <X className="w-5 h-5 text-gray-700" />
              </button>

              {/* Image Section */}
              <div className="flex-1 bg-black flex items-center justify-center relative overflow-hidden">
                <motion.img
                  initial={{ opacity: 0, scale: 1.1 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ duration: 0.4, delay: 0.1 }}
                  src={post.image}
                  alt="Post"
                  className="w-full h-full object-contain"
                />
              </div>

              {/* Comments Sidebar */}
              <div className="w-[420px] flex flex-col bg-white">
                {/* Header */}
                <div className="p-5 border-b border-gray-100 flex items-center gap-3">
                  <motion.img
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
                    src={post.user.avatar}
                    alt={post.user.name}
                    className="w-11 h-11 rounded-full object-cover ring-2 ring-blue-100"
                  />
                  <div className="flex-1">
                    <p
                      className="font-semibold text-gray-900"
                      style={{ fontFamily: "'Sora', sans-serif" }}
                    >
                      {post.user.name}
                    </p>
                    <p className="text-xs text-gray-500">{post.timestamp}</p>
                  </div>
                  <button className="p-2 hover:bg-gray-50 rounded-full transition-colors duration-200">
                    <MoreHorizontal className="w-5 h-5 text-gray-600" />
                  </button>
                </div>

                {/* Comments List */}
                <div className="flex-1 overflow-y-auto p-5 space-y-5">
                  {/* Caption as first comment */}
                  <motion.div
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.3 }}
                    className="flex gap-3"
                  >
                    <img
                      src={post.user.avatar}
                      alt={post.user.name}
                      className="w-9 h-9 rounded-full object-cover flex-shrink-0"
                    />
                    <div className="flex-1">
                      <p className="text-sm">
                        <span
                          className="font-semibold text-gray-900 mr-2"
                          style={{ fontFamily: "'Sora', sans-serif" }}
                        >
                          {post.user.name}
                        </span>
                        <span className="text-gray-700">{post.caption}</span>
                      </p>
                      <p className="text-xs text-gray-400 mt-1">
                        {post.timestamp}
                      </p>
                    </div>
                  </motion.div>

                  {/* Comments */}
                  {post.comments.map((comment, index) => (
                    <motion.div
                      key={comment.id}
                      initial={{ opacity: 0, x: -20 }}
                      animate={{ opacity: 1, x: 0 }}
                      transition={{ delay: 0.3 + index * 0.05 }}
                      className="flex gap-3 group"
                    >
                      <img
                        src={comment.user.avatar}
                        alt={comment.user.name}
                        className="w-9 h-9 rounded-full object-cover flex-shrink-0"
                      />
                      <div className="flex-1">
                        <p className="text-sm">
                          <span
                            className="font-semibold text-gray-900 mr-2"
                            style={{ fontFamily: "'Sora', sans-serif" }}
                          >
                            {comment.user.name}
                          </span>
                          <span className="text-gray-700">{comment.text}</span>
                        </p>
                        <div className="flex items-center gap-4 mt-1">
                          <p className="text-xs text-gray-400">
                            {comment.timestamp}
                          </p>
                          <button className="text-xs text-gray-500 font-medium hover:text-gray-700 transition-colors">
                            {comment.likes} likes
                          </button>
                          <button className="text-xs text-gray-500 font-medium hover:text-gray-700 transition-colors">
                            Reply
                          </button>
                        </div>
                      </div>
                      <button className="opacity-0 group-hover:opacity-100 transition-opacity duration-200 p-1">
                        <Heart className="w-3.5 h-3.5 text-gray-400 hover:text-red-500 transition-colors" />
                      </button>
                    </motion.div>
                  ))}
                </div>

                {/* Actions & Comment Input */}
                <div className="border-t border-gray-100">
                  {/* Action Buttons */}
                  <div className="p-4 flex items-center justify-between">
                    <div className="flex items-center gap-4">
                      <motion.button
                        whileTap={{ scale: 0.9 }}
                        onClick={() => setIsLiked(!isLiked)}
                        className="group"
                      >
                        <Heart
                          className={`w-6 h-6 transition-all duration-200 ${
                            isLiked
                              ? "fill-red-500 text-red-500 scale-110"
                              : "text-gray-700 group-hover:text-red-500 group-hover:scale-110"
                          }`}
                        />
                      </motion.button>
                      <motion.button
                        whileTap={{ scale: 0.9 }}
                        className="group"
                      >
                        <MessageCircle className="w-6 h-6 text-gray-700 group-hover:text-blue-500 transition-colors duration-200" />
                      </motion.button>
                      <motion.button
                        whileTap={{ scale: 0.9 }}
                        className="group"
                      >
                        <Send className="w-6 h-6 text-gray-700 group-hover:text-blue-500 transition-colors duration-200" />
                      </motion.button>
                    </div>
                    <motion.button
                      whileTap={{ scale: 0.9 }}
                      onClick={() => setIsSaved(!isSaved)}
                      className="group"
                    >
                      <Bookmark
                        className={`w-6 h-6 transition-all duration-200 ${
                          isSaved
                            ? "fill-blue-600 text-blue-600"
                            : "text-gray-700 group-hover:text-blue-600"
                        }`}
                      />
                    </motion.button>
                  </div>

                  {/* Likes Count */}
                  <div className="px-4 pb-3">
                    <p
                      className="font-semibold text-sm text-gray-900"
                      style={{ fontFamily: "'Sora', sans-serif" }}
                    >
                      {post.likes.toLocaleString()} likes
                    </p>
                  </div>

                  {/* Comment Input */}
                  <form onSubmit={handleSubmitComment} className="p-4 pt-0">
                    <div className="flex items-center gap-3 border-t border-gray-100 pt-4">
                      <button
                        type="button"
                        className="p-2 hover:bg-gray-50 rounded-full transition-colors duration-200"
                      >
                        <Smile className="w-5 h-5 text-gray-600" />
                      </button>
                      <input
                        type="text"
                        value={newComment}
                        onChange={(e) => setNewComment(e.target.value)}
                        placeholder="Add a comment..."
                        className="flex-1 bg-transparent text-sm outline-none placeholder:text-gray-400"
                      />
                      <button
                        type="submit"
                        disabled={!newComment.trim()}
                        className={`font-semibold text-sm transition-all duration-200 ${
                          newComment.trim()
                            ? "text-blue-600 hover:text-blue-700"
                            : "text-blue-300 cursor-not-allowed"
                        }`}
                        style={{ fontFamily: "'Sora', sans-serif" }}
                      >
                        Post
                      </button>
                    </div>
                  </form>
                </div>
              </div>
            </motion.div>
          </div>
        </>
      )}
    </AnimatePresence>
  );
}
