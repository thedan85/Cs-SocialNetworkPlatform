import { useState, useCallback } from 'react';
import { useApi } from './useApi';
import { Post } from '../types';

export const usePosts = () => {
  const { request, loading, error } = useApi();
  const [posts, setPosts] = useState<Post[]>([]);

  const getPosts = useCallback(async (pageNumber = 1, pageSize = 20) => {
    try {
      const data = await request<Post[]>('get', '/posts', undefined, {
        params: { pageNumber, pageSize }
      });
      setPosts(data);
      return data;
    } catch (err) {
      throw err;
    }
  }, [request]);

  const createPost = useCallback(async (
    content: string,
    imageUrl?: string,
    privacy: 'Public' | 'Friends' | 'Private' = 'Public'
  ) => {
    try {
      const newPost = await request<Post>('post', '/posts', {
        content,
        imageUrl: imageUrl || null,
        privacy
      });
      setPosts((current) => [newPost, ...current]);
      return newPost;
    } catch (err) {
      throw err;
    }
  }, [request]);

  return { posts, getPosts, createPost, loading, error };
};
