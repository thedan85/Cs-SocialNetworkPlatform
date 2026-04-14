import { FormEvent, useEffect, useState } from 'react';
import type { Story } from '../types';
import { createStory, getStories } from '../services/stories';

const Stories = () => {
  const [stories, setStories] = useState<Story[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [content, setContent] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [expiresAt, setExpiresAt] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadStories = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getStories();
      setStories(data);
    } catch (err: any) {
      setError(err.message || 'Failed to load stories.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadStories();
  }, []);

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    if (!content.trim()) return;
    setSubmitting(true);
    try {
      const newStory = await createStory({
        content: content.trim(),
        imageUrl: imageUrl.trim() || null,
        expiresAt: expiresAt ? new Date(expiresAt).toISOString() : null
      });
      setStories((current) => [newStory, ...current]);
      setContent('');
      setImageUrl('');
      setExpiresAt('');
    } catch (err) {
      alert('Unable to create story.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-800">Stories</h1>
        <p className="text-sm text-gray-500">Share quick updates with your friends.</p>
      </div>

      <form onSubmit={handleCreate} className="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-800">Create Story</h2>
        <div className="mt-3 space-y-3">
          <textarea
            value={content}
            onChange={(event) => setContent(event.target.value)}
            rows={3}
            placeholder="What's happening?"
            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-200"
          />
          <input
            value={imageUrl}
            onChange={(event) => setImageUrl(event.target.value)}
            placeholder="Image URL (optional)"
            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-200"
          />
          <div>
            <label className="text-sm font-medium text-gray-700">Expires At</label>
            <input
              type="datetime-local"
              value={expiresAt}
              onChange={(event) => setExpiresAt(event.target.value)}
              className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-200"
            />
          </div>
          <button
            type="submit"
            disabled={submitting}
            className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:bg-gray-400"
          >
            {submitting ? 'Sharing...' : 'Share story'}
          </button>
        </div>
      </form>

      {loading && <div className="text-gray-600">Loading stories...</div>}
      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      <div className="space-y-3">
        {stories.map((story) => (
          <div key={story.storyId} className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
            <div className="flex items-center justify-between">
              <p className="text-sm font-semibold text-gray-800">User {story.userId.slice(0, 8)}</p>
              <p className="text-xs text-gray-400">{new Date(story.createdAt).toLocaleString()}</p>
            </div>
            <p className="mt-2 text-sm text-gray-700">{story.content}</p>
            {story.imageUrl && (
              <img src={story.imageUrl} alt="Story" className="mt-3 w-full rounded-lg object-cover" />
            )}
            <p className="mt-2 text-xs text-gray-400">
              Expires: {new Date(story.expiresAt).toLocaleString()}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Stories;
