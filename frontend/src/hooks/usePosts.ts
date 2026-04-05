import { useState, useCallback } from 'react';
import { useApi } from './useApi';
import { Post } from '../types';

export const usePosts = () => {
  const { request, loading, error } = useApi();
  const [posts, setPosts] = useState<Post[]>([]);

  const getPosts = useCallback(async () => {
    try {
      const data = await request('get', '/posts');
      setPosts(data);
      return data;
    } catch (err) {
      throw err;
    }
  }, [request]);

  const createPost = useCallback(async (content: string, image?: File) => {
    try {
      const formData = new FormData();
      formData.append('content', content);
      if (image) {
        formData.append('image', image);
      }
      
      const newPost = await request('post', '/posts', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      
      setPosts([newPost, ...posts]);
      return newPost;
    } catch (err) {
      throw err;
    }
  }, [request, posts]);

  return { posts, getPosts, createPost, loading, error };
};
