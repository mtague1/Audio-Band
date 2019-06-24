﻿using System;
using System.Collections.Generic;
using AudioBand.AudioSource;
using AudioBand.Messages;
using AudioBand.ViewModels;
using AudioBand.Models;
using Moq;
using AudioBand.Settings;
using Xunit;

namespace AudioBand.Test
{
    /// <summary>
    /// Summary description for CustomLabelsViewModel
    /// </summary>
    public class CustomLabelsViewModelTests
    {
        private Mock<IDialogService> _dialogMock;
        private Mock<IAppSettings> _appSettingsMock;
        private Mock<IAudioSession> _sessionMock;
        private Mock<IMessageBus> _messageBus;
        private CustomLabel _label;
        private CustomLabelsViewModel _viewModel;

        public CustomLabelsViewModelTests()
        {
            _dialogMock = new Mock<IDialogService>();
            _appSettingsMock = new Mock<IAppSettings>();
            _appSettingsMock.Setup(x => x.CustomLabels).Returns(new List<CustomLabel>());
            _label = new CustomLabel();
            _sessionMock = new Mock<IAudioSession>();
            _messageBus = new Mock<IMessageBus>();
            _viewModel = new CustomLabelsViewModel(_appSettingsMock.Object, _dialogMock.Object, _sessionMock.Object, _messageBus.Object);
        }

        [Fact]
        public void AddLabel()
        {
            _viewModel.AddLabelCommand.Execute(null);
            
            Assert.Single(_viewModel.CustomLabels);
        }

        [Fact]
        public void RemoveLabel_confirm()
        {
            _dialogMock.Setup(o => o.ShowConfirmationDialog(It.IsAny<ConfirmationDialogType>(), It.IsAny<object>())).Returns(true);
            _viewModel.AddLabelCommand.Execute(null);
            var newLabel = _viewModel.CustomLabels[0];
            _viewModel.RemoveLabelCommand.Execute(newLabel);

            Assert.Empty(_viewModel.CustomLabels);
            _dialogMock.Verify(o => o.ShowConfirmationDialog(
                It.Is<ConfirmationDialogType>(type => type == ConfirmationDialogType.DeleteLabel),
                It.Is<object[]>(data => data.Length == 1)), 
                Times.Once);
        }

        [Fact]
        public void RemoveLabel_deny()
        {
            _dialogMock.Setup(o => o.ShowConfirmationDialog(It.IsAny<ConfirmationDialogType>(), It.IsAny<object>())).Returns(false);
            _viewModel.AddLabelCommand.Execute(null);
            var newLabel = _viewModel.CustomLabels[0];
            _viewModel.RemoveLabelCommand.Execute(newLabel);

            Assert.Single(_viewModel.CustomLabels);
            _dialogMock.Verify(o => o.ShowConfirmationDialog(
                It.Is<ConfirmationDialogType>(type => type == ConfirmationDialogType.DeleteLabel),
                It.Is<object[]>(data => data.Length == 1)),
                Times.Once);
        }

        [Fact]
        public void AddLabel_ThenCancel()
        {
            _viewModel.BeginEdit();
            _viewModel.AddLabelCommand.Execute(null);
            var newLabel = _viewModel.CustomLabels[0];
            _viewModel.CancelEdit();

            Assert.Empty(_viewModel.CustomLabels);
        }

        [Fact]
        public void RemoveLabel_ThenCancel()
        {
            _appSettingsMock.SetupGet(x => x.CustomLabels).Returns(new List<CustomLabel> { new CustomLabel() });
            _dialogMock.Setup(o => o.ShowConfirmationDialog(It.IsAny<ConfirmationDialogType>(), It.IsAny<object>())).Returns(true);
            _viewModel = new CustomLabelsViewModel(_appSettingsMock.Object, _dialogMock.Object, _sessionMock.Object, _messageBus.Object);
            _viewModel.BeginEdit();
            var label = _viewModel.CustomLabels[0];
            _viewModel.RemoveLabelCommand.Execute(label);
            _viewModel.CancelEdit();

            Assert.Single(_viewModel.CustomLabels);
            Assert.Equal(label, _viewModel.CustomLabels[0]);
        }

        [Fact]
        public void AddRemoveLabel_ThenCancel()
        {
            _viewModel.BeginEdit();
            _dialogMock.Setup(o => o.ShowConfirmationDialog(It.IsAny<ConfirmationDialogType>())).Returns(true);
            _viewModel.AddLabelCommand.Execute(null);
            var newLabel = _viewModel.CustomLabels[0];
            _viewModel.RemoveLabelCommand.Execute(newLabel);
            _viewModel.CancelEdit();

            Assert.Empty(_viewModel.CustomLabels);
        }

        [Fact(Skip = "Unable to setup sequence")]
        public void ProfileChangeRemovesAllLabelsAndAddsNewOnes()
        {
            var settingsMock = new Mock<IAppSettings>();
            settingsMock.SetupSequence(m => m.CustomLabels)
                .Returns(new List<CustomLabel> { new CustomLabel { Name = "test" } })
                .Returns(new List<CustomLabel> { new CustomLabel { Name = "second" } });

            var vm = new CustomLabelsViewModel(settingsMock.Object, new Mock<IDialogService>().Object, _sessionMock.Object, _messageBus.Object);
            Assert.Single(vm.CustomLabels);
            Assert.Equal("test", vm.CustomLabels[0].Name);
            _appSettingsMock.Raise(m => m.ProfileChanged += null, null, EventArgs.Empty);
            Assert.Equal("second", vm.CustomLabels[0].Name);
        }

        [Fact]
        public void ProfileChangeViewModelStillHasCurrentTrackData()
        {
            var settingsMock = new Mock<IAppSettings>();
            settingsMock.SetupSequence(m => m.CustomLabels)
                .Returns(new List<CustomLabel> {new CustomLabel()});
            _sessionMock.SetupGet(m => m.IsPlaying).Returns(true);

            var vm = new CustomLabelsViewModel(settingsMock.Object, new Mock<IDialogService>().Object, _sessionMock.Object, _messageBus.Object);
            _appSettingsMock.Raise(m => m.ProfileChanged += null, null, EventArgs.Empty);
            Assert.True(vm.CustomLabels[0].IsPlaying);
        }
    }
}