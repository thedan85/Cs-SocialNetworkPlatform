import { useState, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { X, Send, Loader2, Image as ImageIcon } from 'lucide-react';

interface FormData {
  content: string;
  image: FileList;
}

const CreatePost = () => {
  const [preview, setPreview] = useState<string | null>(null);
  const fileRef = useRef<HTMLInputElement | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { isSubmitting }
  } = useForm<FormData>();

  const content = watch('content');

  const handleImage = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) setPreview(URL.createObjectURL(file));
  };

  const removeImage = () => {
    setPreview(null);
    if (fileRef.current) fileRef.current.value = '';
  };

  const onSubmit = async (data: FormData) => {
    try {
      // Call API to create post
      await new Promise(res => setTimeout(res, 1000));
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
          <button
            type="button"
            onClick={() => fileRef.current?.click()}
            className="flex items-center gap-2 text-blue-600 hover:text-blue-700 font-medium"
          >
            <ImageIcon className="w-5 h-5" />
            Add Image
          </button>
          <input
            type="file"
            hidden
            {...register('image')}
            ref={fileRef}
            onChange={handleImage}
            accept="image/*"
          />

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

