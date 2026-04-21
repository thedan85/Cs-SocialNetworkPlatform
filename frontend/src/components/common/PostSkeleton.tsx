const PostSkeleton = () => {
  return (
    <div className="bg-white/70 backdrop-blur-xl p-4 rounded-2xl shadow-[0_12px_30px_rgba(15,23,42,0.08)] animate-pulse border border-white/60 dark:bg-slate-900/60 dark:border-slate-800/60">
      <div className="flex items-center gap-3 mb-4">
        <div className="w-10 h-10 bg-slate-200/80 rounded-full dark:bg-slate-700/60" />
        <div className="flex-1">
          <div className="h-3 bg-slate-200/80 rounded w-1/3 mb-2 dark:bg-slate-700/60" />
          <div className="h-2 bg-slate-200/80 rounded w-1/4 dark:bg-slate-700/60" />
        </div>
      </div>

      <div className="space-y-2 mb-4">
        <div className="h-3 bg-slate-200/80 rounded dark:bg-slate-700/60" />
        <div className="h-3 bg-slate-200/80 rounded w-5/6 dark:bg-slate-700/60" />
      </div>

      <div className="h-60 bg-slate-200/80 rounded-xl dark:bg-slate-700/60" />
    </div>
  );
};

export default PostSkeleton;
