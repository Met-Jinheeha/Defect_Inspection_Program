using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace DefectViewProgram
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); //PropertyChangedEventArgs 여기에 프로퍼티 이름을 담아 전달
        } // CallerMemberName = CurrentImage, XAML 요소 변경

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false; // 현재값(필드)와 들어온 값이 같은지 비교
            field = value; // 다르다면 필드 최신화
            OnPropertyChanged(propertyName); // OnPropertyChanged 메서드가 true를 반환하게하여 값이 변경되었음을 알림
            return true;
        }
    }
}
