import { useCallback, useEffect, useState } from 'react'
import postService from '@/services/postService'
import type { Post } from '@/types'

interface UsePostsOptions {
  username?: string   // if set, load user's posts; else load feed
  pageSize?: number
}

export function usePosts({ username, pageSize = 10 }: UsePostsOptions = {}) {
  const [posts,     setPosts]     = useState<Post[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [isError,   setIsError]   = useState(false)
  const [page,      setPage]      = useState(1)
  const [hasMore,   setHasMore]   = useState(true)

  const fetchPosts = useCallback(async (p: number) => {
    setIsLoading(true)
    setIsError(false)
    try {
      const res = username
        ? await postService.getUserPosts(username, p, pageSize)
        : await postService.getFeed(p, pageSize)
      setPosts(prev => p === 1 ? res.items : [...prev, ...res.items])
      setHasMore(res.hasNextPage)
    } catch {
      setIsError(true)
    } finally {
      setIsLoading(false)
    }
  }, [username, pageSize])

  useEffect(() => {
    setPage(1)
    setPosts([])
    fetchPosts(1)
  }, [fetchPosts])

  const loadMore = useCallback(() => {
    if (!hasMore || isLoading) return
    const next = page + 1
    setPage(next)
    fetchPosts(next)
  }, [hasMore, isLoading, page, fetchPosts])

  const refresh = useCallback(() => {
    setPage(1)
    setPosts([])
    fetchPosts(1)
  }, [fetchPosts])

  // Optimistic like toggle
  const toggleLike = useCallback(async (postId: string) => {
    setPosts(prev => prev.map(p =>
      p.id !== postId ? p : {
        ...p,
        isLiked:    !p.isLiked,
        likesCount: p.isLiked ? p.likesCount - 1 : p.likesCount + 1,
      }
    ))
    try {
      const post = posts.find(p => p.id === postId)
      if (!post) return
      post.isLiked
        ? await postService.unlikePost(postId)
        : await postService.likePost(postId)
    } catch {
      // Revert on error
      setPosts(prev => prev.map(p =>
        p.id !== postId ? p : {
          ...p,
          isLiked:    !p.isLiked,
          likesCount: p.isLiked ? p.likesCount - 1 : p.likesCount + 1,
        }
      ))
    }
  }, [posts])

  // Optimistic save toggle
  const toggleSave = useCallback(async (postId: string) => {
    setPosts(prev => prev.map(p =>
      p.id !== postId ? p : { ...p, isSaved: !p.isSaved }
    ))
    try {
      const post = posts.find(p => p.id === postId)
      if (!post) return
      post.isSaved
        ? await postService.unsavePost(postId)
        : await postService.savePost(postId)
    } catch {
      setPosts(prev => prev.map(p =>
        p.id !== postId ? p : { ...p, isSaved: !p.isSaved }
      ))
    }
  }, [posts])

  const addPost = useCallback((post: Post) => {
    setPosts(prev => [post, ...prev])
  }, [])

  const removePost = useCallback(async (postId: string) => {
    setPosts(prev => prev.filter(p => p.id !== postId))
    try {
      await postService.deletePost(postId)
    } catch {
      refresh()
    }
  }, [refresh])

  return { posts, isLoading, isError, hasMore, loadMore, refresh, toggleLike, toggleSave, addPost, removePost }
}
