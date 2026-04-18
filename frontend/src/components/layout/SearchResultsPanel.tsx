import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import type { HashtagSearchResult, User } from '../../types';

interface SearchResultsPanelProps {
  query: string;
  searching: boolean;
  error?: string | null;
  users: User[];
  hashtags: HashtagSearchResult[];
  onSelectHistory: (value: string) => void;
  history: string[];
  onClearHistory: () => void;
}

const SearchResultsPanel: React.FC<SearchResultsPanelProps> = ({
  query,
  searching,
  error,
  users,
  hashtags,
  onSelectHistory,
  history,
  onClearHistory
}) => {
  const trimmed = query.trim();
  const { user: currentUser } = useAuth();

  if (!trimmed) {
    return (
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <p className="text-sm font-semibold uppercase tracking-wide text-slate-400">Recent searches</p>
          {history.length > 0 && (
            <button
              type="button"
              onClick={onClearHistory}
              className="text-xs font-medium text-slate-500 hover:text-slate-800 dark:text-slate-400 dark:hover:text-slate-100"
            >
              Clear
            </button>
          )}
        </div>
        {history.length === 0 ? (
          <p className="text-sm text-slate-500 dark:text-slate-400">No search history yet.</p>
        ) : (
          <div className="space-y-2">
            {history.map((item) => (
              <button
                key={item}
                type="button"
                onClick={() => onSelectHistory(item)}
                className="w-full text-left rounded-xl border border-white/60 bg-white/70 px-4 py-3 text-base font-medium text-slate-700 shadow-sm hover:bg-white dark:border-slate-800/60 dark:bg-slate-900/60 dark:text-slate-200 dark:hover:bg-slate-800/70"
              >
                {item}
              </button>
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {searching && <p className="text-sm text-slate-500 dark:text-slate-400">Searching...</p>}
      {error && (
        <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-3 py-2 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {error}
        </div>
      )}
      {!searching && !error && users.length === 0 && hashtags.length === 0 && (
        <p className="text-sm text-slate-500 dark:text-slate-400">No results found.</p>
      )}

      {users.length > 0 && (
        <div className="space-y-2">
          <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">Users</p>
          {users.map((user) => {
            const fullName = [user.firstName, user.lastName].filter(Boolean).join(' ').trim();
            const profilePath = currentUser?.userId === user.userId
              ? '/profile'
              : `/users/${user.userId}`;
            return (
              <Link
                key={user.userId}
                to={profilePath}
                className="rounded-lg border border-white/60 bg-white/70 px-3 py-2 dark:border-slate-800/60 dark:bg-slate-900/60"
              >
                <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">
                  {fullName || user.userName}
                </p>
                <p className="text-xs text-slate-500 dark:text-slate-400">@{user.userName}</p>
              </Link>
            );
          })}
        </div>
      )}

      {hashtags.length > 0 && (
        <div className="space-y-2">
          <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">Hashtags</p>
          {hashtags.map((tag) => (
            <div
              key={tag.hashtagId}
              className="rounded-xl border border-white/60 bg-white/70 px-3 py-3 dark:border-slate-800/60 dark:bg-slate-900/60"
            >
              <div className="flex items-center justify-between">
                <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">{tag.tag}</p>
                <span className="text-xs text-slate-500 dark:text-slate-400">
                  {tag.usageCount} uses
                </span>
              </div>
              {tag.posts.length === 0 ? (
                <p className="mt-2 text-xs text-slate-500 dark:text-slate-400">No posts using this hashtag yet.</p>
              ) : (
                <div className="mt-2 space-y-2">
                  {tag.posts.map((post) => {
                    const name = [post.firstName, post.lastName].filter(Boolean).join(' ').trim() || post.userName;
                    const postProfilePath = currentUser?.userId === post.userId
                      ? '/profile'
                      : `/users/${post.userId}`;
                    const snippet = post.content.length > 140
                      ? `${post.content.slice(0, 140)}...`
                      : post.content;
                    return (
                      <div key={post.postId} className="rounded-lg bg-white/80 px-3 py-2 text-xs text-slate-600 dark:bg-slate-950/60 dark:text-slate-300">
                        <Link
                          to={postProfilePath}
                          className="font-semibold text-slate-700 hover:text-cyan-700 dark:text-slate-200 dark:hover:text-cyan-200"
                        >
                          {name || 'Unknown user'}
                        </Link>
                        <p>{snippet}</p>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default SearchResultsPanel;
