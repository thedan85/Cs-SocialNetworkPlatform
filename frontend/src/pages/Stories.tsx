import { ChangeEvent, FormEvent, useEffect, useId, useRef, useState } from 'react';
import type { Story } from '../types';
import { createStory, getStories } from '../services/stories';
import { uploadImage } from '../services/uploads';

const Stories = () => {
  const [stories, setStories] = useState<Story[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [content, setContent] = useState('');
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [expiresAt, setExpiresAt] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [uploadingImage, setUploadingImage] = useState(false);
  const inputId = useId();
  const fileInputRef = useRef<HTMLInputElement | null>(null);

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

  useEffect(() => {
    return () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    };
  }, [previewUrl]);

  const handleImageChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) {
      return;
    }

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    setImageFile(file);
    setPreviewUrl(URL.createObjectURL(file));
  };

  const removeImage = () => {
    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    setPreviewUrl(null);
    setImageFile(null);

    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    if (!content.trim()) return;
    setSubmitting(true);
    setUploadingImage(!!imageFile);
    try {
      let imageUrl: string | null = null;
      if (imageFile) {
        const uploaded = await uploadImage(imageFile);
        imageUrl = uploaded.url;
      }

      const newStory = await createStory({
        content: content.trim(),
        imageUrl,
        expiresAt: expiresAt ? new Date(expiresAt).toISOString() : null
      });
      setStories((current) => [newStory, ...current]);
      setContent('');
      removeImage();
      setExpiresAt('');
    } catch (err) {
      alert('Unable to create story.');
    } finally {
      setUploadingImage(false);
      setSubmitting(false);
    }
  };

  const buildDisplayName = (
    firstName?: string | null,
    lastName?: string | null,
    userName?: string | null,
    fallbackId?: string
  ) => {
    const fullName = [firstName, lastName].filter(Boolean).join(' ').trim();
    if (fullName) return fullName;
    if (userName) return userName;
    if (fallbackId) return `User ${fallbackId.slice(0, 8)}`;
    return 'Unknown user';
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight dark:text-slate-100">Stories</h1>
        <p className="text-sm text-slate-500 dark:text-slate-400">Share quick updates with your friends.</p>
      </div>

      <form onSubmit={handleCreate} className="rounded-2xl border border-white/60 bg-white/70 backdrop-blur-xl p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:bg-slate-900/60 dark:border-slate-800/60">
        <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Create Story</h2>
        <div className="mt-3 space-y-3">
          <textarea
            value={content}
            onChange={(event) => setContent(event.target.value)}
            rows={3}
            placeholder="What's happening?"
            className="w-full rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
          />
          <div className="flex flex-col gap-2">
            <label
              htmlFor={inputId}
              className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-white transition-colors cursor-pointer dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-200 dark:hover:bg-slate-800/80"
            >
              {imageFile ? 'Change image' : 'Attach image'}
            </label>
            <input
              ref={fileInputRef}
              id={inputId}
              type="file"
              accept="image/*"
              onChange={handleImageChange}
              className="hidden"
            />
            {previewUrl && (
              <div className="relative">
                <img src={previewUrl} alt="Preview" className="w-full rounded-xl object-cover" />
                <button
                  type="button"
                  onClick={removeImage}
                  className="absolute top-2 right-2 rounded-full bg-rose-500 p-1 text-white hover:bg-rose-600"
                >
                  Remove
                </button>
              </div>
            )}
          </div>
          <div>
            <label className="text-sm font-medium text-slate-600 dark:text-slate-300">Expires At</label>
            <input
              type="datetime-local"
              value={expiresAt}
              onChange={(event) => setExpiresAt(event.target.value)}
              className="mt-1 w-full rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:focus:ring-cyan-500/40"
            />
          </div>
          <button
            type="submit"
            disabled={submitting || uploadingImage}
            className="rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-cyan-500/25 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:from-slate-300 disabled:via-slate-300 disabled:to-slate-300 disabled:text-slate-500 disabled:shadow-none"
          >
            {uploadingImage ? 'Uploading...' : submitting ? 'Sharing...' : 'Share story'}
          </button>
        </div>
      </form>

      {loading && <div className="text-slate-600 dark:text-slate-400">Loading stories...</div>}
      {error && (
        <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {error}
        </div>
      )}

      <div className="space-y-3">
        {stories.map((story) => (
          <div key={story.storyId} className="rounded-2xl border border-white/60 bg-white/70 backdrop-blur-xl p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:bg-slate-900/60 dark:border-slate-800/60">
            <div className="flex items-center justify-between">
              <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">
                {buildDisplayName(story.firstName, story.lastName, story.userName, story.userId)}
              </p>
              <p className="text-xs text-slate-400 dark:text-slate-500">{new Date(story.createdAt).toLocaleString()}</p>
            </div>
            <p className="mt-2 text-sm text-slate-700 dark:text-slate-200">{story.content}</p>
            {story.imageUrl && (
              <img src={story.imageUrl} alt="Story" className="mt-3 w-full rounded-lg object-cover" />
            )}
            <p className="mt-2 text-xs text-slate-400 dark:text-slate-500">
              Expires: {new Date(story.expiresAt).toLocaleString()}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Stories;
