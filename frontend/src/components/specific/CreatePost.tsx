import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X, Send, Loader2, Image as ImageIcon } from 'lucide-react';

interface FormData {
  content: string;
  imageUrl?: string;
}

interface CreatePostProps {
  onCreate: (content: string, imageUrl?: string) => Promise<unknown>;
}

const CreatePost: React.FC<CreatePostProps> = ({ onCreate }) => {
  const [preview, setPreview] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { isSubmitting }
  } = useForm<FormData>();

  const content = watch('content');
  const imageUrl = watch('imageUrl');

  useEffect(() => {
    if (imageUrl && imageUrl.trim()) {
      setPreview(imageUrl.trim());
    } else {
      setPreview(null);
    }
  }, [imageUrl]);

  const removeImage = () => {
    setPreview(null);
    setValue('imageUrl', '');
  };

  const onSubmit = async (data: FormData) => {
    try {
      await onCreate(data.content, data.imageUrl?.trim() || undefined);
      reset();
      setPreview(null);
      alert('Post created successfully!');
    } catch (err) {
      alert('Error creating post');
    }
  };

  return (
    <div className="bg-white p-4 rounded-xl shadow-sm border border-gray-100">
      <form onSubmit={handleSubmit(onSubmit)}>
        <textarea
          {...register('content', { required: true })}
          placeholder="What's on your mind?"
          className="w-full p-3 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 resize-none"
          rows={4}
        />

        {preview && (
          <div className="relative mt-3">
            <img src={preview} alt="Preview" className="rounded-lg w-full max-h-80 object-cover" />
            <button 
              onClick={removeImage} 
              type="button" 
              className="absolute top-2 right-2 bg-red-500 hover:bg-red-600 text-white p-1 rounded-full"
            >
              <X className="w-5 h-5" />
            </button>
          </div>
        )}

        <div className="flex justify-between items-center mt-3">
          <div className="flex-1 mr-3">
            <div className="relative">
              <ImageIcon className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
              <input
                type="url"
                placeholder="Image URL (optional)"
                className="w-full rounded-lg border border-gray-200 bg-gray-50 py-2 pl-9 pr-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                {...register('imageUrl')}
              />
            </div>
          </div>

          <button 
            type="submit" 
            disabled={!content || isSubmitting}
            className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold px-4 py-2 rounded-lg disabled:bg-gray-400 transition-colors"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="w-4 h-4 animate-spin" />
                Posting...
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

