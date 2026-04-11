import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useForm } from 'react-hook-form';
import Input from '../components/common/Input';
import { User, LogOut } from 'lucide-react';

interface UpdateProfileForm {
  username: string;
  email: string;
  avatar?: FileList;
}

const Profile = () => {
  const { user, logout } = useAuth();
  const { register, handleSubmit, formState: { isSubmitting } } = useForm<UpdateProfileForm>(
    {
      defaultValues: {
        username: user?.username || '',
        email: user?.email || '',
      },
    }
  );
  const [preview, setPreview] = useState<string | null>(user?.avatarUrl || null);
  const [isEditing, setIsEditing] = useState(false);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setPreview(URL.createObjectURL(file));
    }
  };

  const onSubmit = async (data: UpdateProfileForm) => {
    try {
      console.log('Update profile:', data);
      // Call API: await api.put('/auth/profile', data)
      await new Promise(resolve => setTimeout(resolve, 1000));
      alert('Profile updated successfully!');
      setIsEditing(false);
    } catch (err) {
      alert('Error updating profile');
    }
  };

  const handleLogout = () => {
    logout();
  };

  return (
    <div className="max-w-2xl mx-auto py-8">
      <div className="bg-white rounded-xl shadow-lg p-8">
        {/* Profile Header */}
        <div className="flex items-center gap-6 mb-8 pb-8 border-b">
          <div className="w-24 h-24 rounded-full bg-gradient-to-br from-blue-400 to-blue-600 flex items-center justify-center flex-shrink-0">
            {preview ? (
              <img src={preview} alt="Avatar" className="w-full h-full rounded-full object-cover" />
            ) : (
              <User className="w-12 h-12 text-white" />
            )}
          </div>
          <div className="flex-1">
            <h1 className="text-3xl font-bold text-gray-800">{user?.username}</h1>
            <p className="text-gray-600 mt-2">{user?.email}</p>
          </div>
          <button
            onClick={handleLogout}
            className="flex items-center gap-2 bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg font-medium transition-colors"
          >
            <LogOut className="w-4 h-4" />
            Logout
          </button>
        </div>

        {/* Profile Info Section */}
        <div className="mb-8">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-bold text-gray-800">Profile Information</h2>
            <button
              onClick={() => setIsEditing(!isEditing)}
              className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition-colors"
            >
              {isEditing ? 'Cancel' : 'Edit Profile'}
            </button>
          </div>

          {isEditing ? (
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="flex flex-col items-center gap-2 mb-6">
                <div className="w-24 h-24 rounded-full bg-gray-200 overflow-hidden border-2 border-blue-500">
                  {preview ? (
                    <img src={preview} alt="Preview" className="w-full h-full object-cover" />
                  ) : (
                    <div className="flex items-center justify-center h-full text-gray-400">No Avatar</div>
                  )}
                </div>
                <label className="text-sm text-blue-600 cursor-pointer hover:underline font-medium">
                  Change Avatar
                  <input
                    type="file"
                    className="hidden"
                    accept="image/*"
                    {...register('avatar')}
                    onChange={handleFileChange}
                  />
                </label>
              </div>

              <Input
                label="Username"
                placeholder="Your username"
                {...register('username')}
              />

              <Input
                label="Email"
                type="email"
                placeholder="your.email@example.com"
                {...register('email')}
              />

              <button
                type="submit"
                disabled={isSubmitting}
                className="w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-2 rounded-lg transition-colors disabled:bg-gray-400"
              >
                {isSubmitting ? 'Saving...' : 'Save Changes'}
              </button>
            </form>
          ) : (
            <div className="space-y-4">
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Username</p>
                <p className="text-lg font-semibold text-gray-800">{user?.username}</p>
              </div>
              <div className="bg-gray-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Email</p>
                <p className="text-lg font-semibold text-gray-800">{user?.email}</p>
              </div>
            </div>
          )}
        </div>

        {/* Account Stats */}
        <div className="grid grid-cols-3 gap-4">
          <div className="bg-blue-50 p-4 rounded-lg text-center">
            <p className="text-2xl font-bold text-blue-600">0</p>
            <p className="text-sm text-gray-600">Posts</p>
          </div>
          <div className="bg-green-50 p-4 rounded-lg text-center">
            <p className="text-2xl font-bold text-green-600">0</p>
            <p className="text-sm text-gray-600">Followers</p>
          </div>
          <div className="bg-purple-50 p-4 rounded-lg text-center">
            <p className="text-2xl font-bold text-purple-600">0</p>
            <p className="text-sm text-gray-600">Following</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile;
