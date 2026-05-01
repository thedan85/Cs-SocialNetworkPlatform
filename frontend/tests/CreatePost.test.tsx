import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import CreatePost from '../src/components/specific/CreatePost';
import { uploadImage } from '../src/services/uploads';

vi.mock('../src/services/uploads', () => ({
  uploadImage: vi.fn()
}));

const uploadImageMock = vi.mocked(uploadImage);

describe('CreatePost', () => {
  afterEach(() => {
    vi.clearAllMocks();
  });

  it('submits content and privacy', async () => {
    const onCreate = vi.fn().mockResolvedValue(undefined);
    const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {});

    render(<CreatePost onCreate={onCreate} />);

    await userEvent.type(screen.getByPlaceholderText("What's on your mind?"), 'Hello there');
    await userEvent.selectOptions(screen.getByRole('combobox'), 'Friends');
    await userEvent.click(screen.getByRole('button', { name: /post/i }));

    await waitFor(() =>
      expect(onCreate).toHaveBeenCalledWith('Hello there', undefined, 'Friends')
    );
    expect(alertSpy).toHaveBeenCalledWith('Post created successfully!');

    alertSpy.mockRestore();
  });

  it('uploads image before submitting', async () => {
    const onCreate = vi.fn().mockResolvedValue(undefined);
    const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {});
    uploadImageMock.mockResolvedValue({ url: 'https://cdn.test/photo.png' });

    render(<CreatePost onCreate={onCreate} />);

    const file = new File(['image'], 'photo.png', { type: 'image/png' });
    const fileInput = screen.getByLabelText(/attach image/i) as HTMLInputElement;

    await userEvent.upload(fileInput, file);
    await userEvent.type(screen.getByPlaceholderText("What's on your mind?"), 'With image');
    await userEvent.click(screen.getByRole('button', { name: /post/i }));

    await waitFor(() => expect(uploadImageMock).toHaveBeenCalledWith(file));
    await waitFor(() =>
      expect(onCreate).toHaveBeenCalledWith('With image', 'https://cdn.test/photo.png', 'Public')
    );

    alertSpy.mockRestore();
  });
});
