import { ChangeEvent, useEffect, useId, useRef, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X, Send, Loader2, Image as ImageIcon } from 'lucide-react';
import { uploadImage } from '../../services/uploads';

interface FormData {
  content: string;
  privacy: 'Public' | 'Friends' | 'Private';
}

interface CreatePostProps {
  onCreate: (content: string, imageUrl?: string, privacy?: 'Public' | 'Friends' | 'Private') => Promise<unknown>;
}

const CreatePost: React.FC<CreatePostProps> = ({ onCreate }) => {
  const inputId = useId();
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [uploadingImage, setUploadingImage] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { isSubmitting }
  } = useForm<FormData>({
    defaultValues: {
      privacy: 'Public'
    }
  });

  const content = watch('content');
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

  const onSubmit = async (data: FormData) => {
    setUploadingImage(!!imageFile);
    try {
      let imageUrl: string | undefined;
      if (imageFile) {
        const uploaded = await uploadImage(imageFile);
        imageUrl = uploaded.url;
      }

      await onCreate(data.content, imageUrl, data.privacy);
      reset();
      removeImage();
      alert('Post created successfully!');
    } catch (err) {
      alert('Error creating post');
    } finally {
      setUploadingImage(false);
    }
  };

  const isBusy = isSubmitting || uploadingImage;

  return (
    <div className="bg-white/70 backdrop-blur-xl p-5 rounded-2xl shadow-[0_16px_40px_rgba(15,23,42,0.08)] border border-white/60 dark:bg-slate-900/60 dark:border-slate-800/60">
      <form onSubmit={handleSubmit(onSubmit)}>
        <textarea
          {...register('content', { required: true })}
          placeholder="What's on your mind?"
          className="w-full p-3 rounded-xl border border-white/70 bg-white/80 text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 resize-none dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
          rows={4}
        />

        {previewUrl && (
          <div className="relative mt-3">
            <img src={previewUrl} alt="Preview" className="rounded-lg w-full max-h-80 object-cover" />
            <button 
              onClick={removeImage} 
              type="button" 
              className="absolute top-2 right-2 bg-rose-500 hover:bg-rose-600 text-white p-1 rounded-full"
            >
              <X className="w-5 h-5" />
            </button>
          </div>
        )}

        <div className="flex flex-col gap-3 mt-3 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex-1">
            <label
              htmlFor={inputId}
              className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-white transition-colors cursor-pointer dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-200 dark:hover:bg-slate-800/80"
            >
              <ImageIcon className="w-4 h-4 text-slate-400 dark:text-slate-500" />
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
          </div>
          <div className="sm:min-w-[160px]">
            <label className="text-xs font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400">
              Privacy
            </label>
            <select
              {...register('privacy')}
              className="mt-2 w-full rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm font-medium text-slate-700 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:focus:ring-cyan-500/40"
            >
              <option value="Public">Public</option>
              <option value="Friends">Friends</option>
              <option value="Private">Private</option>
            </select>
          </div>
          <button 
            type="submit" 
            disabled={!content || isBusy}
            className="flex items-center gap-2 rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-white font-semibold shadow-lg shadow-cyan-500/30 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:from-slate-300 disabled:via-slate-300 disabled:to-slate-300 disabled:text-slate-500 disabled:shadow-none transition-all"
          >
            {isBusy ? (
              <>
                <Loader2 className="w-4 h-4 animate-spin" />
                {uploadingImage ? 'Uploading...' : 'Posting...'}
              </>
            ) : (
              <>
                <Send className="w-4 h-4" />
                Post
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreatePost;

