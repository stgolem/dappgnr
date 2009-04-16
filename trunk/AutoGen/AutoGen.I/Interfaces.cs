using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using AutoGen.TeXML;

namespace AutoGen.I
{
    public static class AutoGenInterfaces
    {
        private static readonly string iAutoGenPlugin = "AutoGen.I.IAutoGenPlugin";
        private static readonly string iAutoGenApplication = "AutoGen.I.IAutoGenApplication";
        private static readonly string iAutoGenGenerator = "AutoGen.I.IAutoGenGenerator";
        private static readonly string iAutoGenTask = "AutoGen.I.IAutoGenTask";
        private static readonly string iAutoGenTaskControl = "AutoGen.I.ITaskControl";
        private static readonly string iAutoGenPrinter = "AutoGen.I.IAutoGenPrinter";

        public static string IAutoGenPlugin
        {
            get { return iAutoGenPlugin; }
        }

        public static string IAutoGenApplication
        {
            get { return iAutoGenApplication; }
        }

        public static string IAutoGenTask
        {
            get { return iAutoGenTask; }
        }

        public static string IAutoGenTaskControl
        {
            get { return iAutoGenTaskControl; }
        }

        public static string IAutoGenGenerator
        {
            get { return iAutoGenGenerator; }
        }

        public static string IAutoGenPrinter
        {
            get { return iAutoGenPrinter; }
        }
    }

    /// <summary>
    /// Интерфейс параметров генерации
    /// </summary>
    public interface IAutoGenParameters
    {
        /// <summary>
        /// Количество заданий в варианте
        /// </summary>
        int CountInVariant { get; set; }
        /// <summary>
        /// Количество вариантов
        /// </summary>
        int Variants { get; set; }
        /// <summary>
        /// Нужен ли ответ на задачу
        /// </summary>
        bool NeedAnswer { get; set; }
        /// <summary>
        /// Название задачи
        /// </summary>
        string TaskName { get; set; }
    }
    /// <summary>
    /// Интерфейс обеспечивающий чтение и запись 
    /// текстовых данных в конфигурационный файл
    /// </summary>
    public interface IAutoGenDBSettings
    {
        /// <summary>
        /// Сохранить данные
        /// </summary>
        /// <param name="set">Имя набора данных</param>
        /// <param name="name">Название</param>
        /// <param name="value">Данные</param>
        void SaveValue(string set, string name, string value);
        /// <summary>
        /// Получить данные
        /// </summary>
        /// <param name="set">Имя набора данных</param>
        /// <param name="name">Название</param>
        /// <returns>Данные</returns>
        string GetValue(string set, string name);
    }
    /// <summary>
    /// Интерфейс главного приложения
    /// </summary>
    public interface IAutoGenApplication
    {
        /// <summary>
        /// Предоставляет объект доступа к настройкам
        /// </summary>
        IAutoGenDBSettings MainDBSettings { get; }
        /// <summary>
        /// Главная форма программы
        /// </summary>
        object MainForm { get; }
        /// <summary>
        /// Директория приложения
        /// </summary>
        string MainApplicationDir { get; }
        /// <summary>
        /// Поддиректория \ТЕХ
        /// </summary>
        string MainTexDir { get; }
        /// <summary>
        /// Расположение портативной версии ТеХ
        /// </summary>
        string MainTexPortDir { get; }
        /// <summary>
        /// Расположение файлов данных
        /// </summary>
        string MainDataDir { get; }
    }
    /// <summary>
    /// Интерфейс плагина - дополнения
    /// </summary>
    public interface IAutoGenPlugin
    {
        /// <summary>
        /// Версия
        /// </summary>
        Version PluginVersion { get; }
        /// <summary>
        /// Автор
        /// </summary>
        string Autor { get; }
        /// <summary>
        /// Короткое название
        /// </summary>
        string PluginName { get; }
        /// <summary>
        /// Произвести инициализацию объекта плагина
        /// </summary>
        /// <param name="autoGenApp">Главное приложение</param>
        void InitPlugin(IAutoGenApplication autoGenApp);
        /// <summary>
        /// Отобразить информацию о плагине
        /// </summary>
        void ShowAbout();
        /// <summary>
        /// Получить уникальный идентификатор плагина
        /// </summary>
        Guid GUID { get; }
    }
    /// <summary>
    /// Интерфейс дополнения - генератора
    /// </summary>
    public interface IAutoGenGenerator : IAutoGenPlugin
    {
        /// <summary>
        /// Получить новый объект Задачи
        /// </summary>
        /// <param name="taskName">Имя задачи</param>
        /// <returns>Объект Задачи для редактирования</returns>
        IAutoGenTask CreateTaskInstance(string taskName);
        /// <summary>
        /// Дополнительные данные плагина
        /// </summary>
        Object GeneratorData { get; set; }
    }
    /// <summary>
    /// Интерфейс экземпляра Задачи для генерации
    /// </summary>
    public interface IAutoGenTask : ISerializable
    {
        /// <summary>
        /// Название
        /// </summary>
        string TaskName { get; set;}
        /// <summary>
        /// Описание
        /// </summary>
        string TaskDescription { get; set; }
        /// <summary>
        /// Объект элемента управления
        /// </summary>
        ITaskControl TaskPropertiesControl { get;}
        /// <summary>
        /// Произвести генерацию задачи
        /// </summary>
        /// <param name="Parameters">Параметры</param>
        /// <param name="Worker">Обработчик</param>
        /// <returns>Объект формата TeXML</returns>
        TeXMLDocList GenerateTask(IAutoGenParameters Parameters, IAutoGenWorker Worker);
    }
    /// <summary>
    /// Интерфейс элемента управления задачей
    /// </summary>
    public interface ITaskControl
    {
        /// <summary>
        /// Основной элемент, управляющий данными задачи
        /// </summary>
        Control InnerControl { get; }

        /// <summary>
        /// Функция вызывается перед закрытием вкладки
        /// </summary>
        /// <param name="sender">Главная форма</param>
        /// <param name="e">Аргументы</param>
        void ParentTabClosing(object sender, CancelEventArgs e);
        /// <summary>
        /// Событие происходяшие в момент сохранения задачи
        /// </summary>
        event TaskChangeEventHandler TaskSaved;
        /// <summary>
        /// Событие происходящие в момент изменения задачи
        /// </summary>
        event TaskChangeEventHandler TaskChanged;
    }
    /// <summary>
    /// Интерфейс модуля принтера
    /// </summary>
    public interface IAutoGenPrinter : IAutoGenPlugin
    {
        /// <summary>
        /// Запустить печать документа
        /// </summary>
        /// <param name="TeXDocumentList">Документ в формате TeXML</param>
        /// <param name="Worker">Обработчик</param>
        /// <param name="Parameters">Параметры</param>
        void Print(TeXMLDocList TeXDocumentList, IAutoGenWorker Worker, IAutoGenParameters Parameters);
        /// <summary>
        /// Отобразить настройки модуля
        /// </summary>
        void ShowProperties();
    }
    /// <summary>
    /// Интерфейс потокового обработчика заданий
    /// </summary>
    public interface IAutoGenWorker
    {
        /// <summary>
        /// Сообщить о выполнении
        /// </summary>
        /// <param name="ProgressPercent">Процент выполнения</param>
        /// <param name="ProgressCaption">Стадия выполнения</param>
        void ReportProgress(int ProgressPercent, string ProgressCaption);
        /// <summary>
        /// Записать сообщение в лог
        /// </summary>
        /// <param name="line">Строка сообщения</param>
        void WriteOutputLine(string line);
        /// <summary>
        /// Послать сигнал отмены выполнения
        /// </summary>
        void CancelProgress();
        /// <summary>
        /// Происходит, когда кто-либо из участников
        /// посылает сигнал отмены
        /// </summary>
        event EventHandler CancelSend;
        /// <summary>
        /// Признак того, что запущена отмена
        /// </summary>
        bool IsCanceled { get; }
    }
}
