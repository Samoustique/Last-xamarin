﻿using System;
using System.IO;
using Last.Core.Models;
using Last.Core.Services;
using Xamarin.Forms;

namespace Last.Core.ViewModels
{
    public abstract class ItemDetailViewModel : BaseViewModel
    {
        public string ButtonTitle { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public ImageSource Image
        {
            get { return this._image; }
            set
            {
                SetProperty(ref _image, value);
            }
        }

        public INavigation Navigation { get; internal set; }
        public Command MainCommand { get; set; }
        public Command PickPhotoButtonCommand { get; set; }
        public Command DeleteItemCommand { get; set; }

        private ImageSource _image;

        public ItemDetailViewModel()
        {
            PickPhotoButtonCommand = new Command(PickPhotoButtonExecute);
            DeleteItemCommand = new Command<Item>(DeleteItemExecute, DeleteItemCanExecute);
        }

        protected bool DeleteItemCanExecute(Item item)
        {
            return item != null;
        }

        private async void DeleteItemExecute(Item item)
        {
            await Navigation.PopAsync();
            MessagingCenter.Send(this, "Delete", item.Id);
        }

        private void PickPhotoButtonExecute()
        {
            var photoPickerService = DependencyService.Get<IPhotoPickerService>();
            SubscribePhotoPicker(photoPickerService);
            photoPickerService.GetImageStreamAsync();
        }

        private void OnPhotoPickedSucceeded(Stream stream)
        {
            UnsubscribePhotoPicker();
            if (stream != null)
            {
                Image = ImageSource.FromStream(() => stream);
            }
        }

        private void OnPhotoPickedFailed()
        {
            UnsubscribePhotoPicker();
            // TODO display message "you need to grant rights"
         }

        private void SubscribePhotoPicker(IPhotoPickerService photoPickerService)
        {
            photoPickerService.PhotoPickedSucceeded += OnPhotoPickedSucceeded;
            photoPickerService.PhotoPickedFailed += OnPhotoPickedFailed;
        }

        private void UnsubscribePhotoPicker()
        {
            var photoPickerService = DependencyService.Get<IPhotoPickerService>();
            photoPickerService.PhotoPickedSucceeded -= OnPhotoPickedSucceeded;
            photoPickerService.PhotoPickedFailed -= OnPhotoPickedFailed;
        }
    }
}
