const PostSkeleton = () => {
  return (
    <div className="bg-white p-4 rounded-xl shadow-sm animate-pulse">
      <div className="flex items-center gap-3 mb-4">
        <div className="w-10 h-10 bg-gray-200 rounded-full" />
        <div className="flex-1">
          <div className="h-3 bg-gray-200 rounded w-1/3 mb-2" />
          <div className="h-2 bg-gray-200 rounded w-1/4" />
        </div>
      </div>

      <div className="space-y-2 mb-4">
        <div className="h-3 bg-gray-200 rounded" />
        <div className="h-3 bg-gray-200 rounded w-5/6" />
      </div>

      <div className="h-60 bg-gray-200 rounded-lg" />
    </div>
  );
};

export default PostSkeleton;
