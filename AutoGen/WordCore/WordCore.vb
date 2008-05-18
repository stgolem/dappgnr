''' <summary>
''' Класс для работы с приложением Microsoft Word
''' </summary>
''' <remarks></remarks>
Public Class WordCore
    Private pID = "Word.Application"
    Private core = DBNull.Value
    Private thisApplication = DBNull.Value
    Private thisDocument = DBNull.Value
    Private docOpened As Boolean = False
    ''' <summary>
    ''' Создает новый объект MSWord
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        core = CreateObject(pID)
    End Sub
    ''' <summary>
    ''' Создает новый документ в приложении.
    ''' </summary>
    ''' <remarks>Для закрытия пользуемся CloseDoc()</remarks>
    Public Sub CreateDoc()
        If docOpened = False Then
            Dim template = Type.Missing
            Dim newTemplate = Type.Missing
            Dim documentType = Type.Missing
            Dim visible = Type.Missing
            Dim file = 1
            thisApplication.Documents.Add(template, newTemplate, documentType, visible)
            thisDocument = thisApplication.Documents.Item(file)
            docOpened = True
        End If
    End Sub
    ''' <summary>
    ''' Открывает приложение MSWord в памяти
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub OpenApp()
        thisApplication = core.Application
    End Sub
    ''' <summary>
    ''' Закывает приложение и выгружает из памяти
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseApp()
        Dim saveChanges = 0 'wdDoNotSaveChanges
        Dim originalFormat = Type.Missing
        Dim routeDocument = Type.Missing
        thisApplication.Quit(saveChanges, originalFormat, routeDocument)
        thisApplication = DBNull.Value
        thisDocument = DBNull.Value
        docOpened = False
    End Sub
    ''' <summary>
    ''' Сохраняет текущий документ
    ''' </summary>
    ''' <param name="FName">Полный путь и имя файла</param>
    ''' <remarks></remarks>
    Public Sub SaveAsDoc(ByVal FName As String)
        Dim fileName = FName
        Dim fileFormat = Type.Missing
        Dim lockComments = Type.Missing
        Dim password = Type.Missing
        Dim addToRecentFiles = Type.Missing
        Dim writePassword = Type.Missing
        Dim readOnlyRecommended = Type.Missing
        Dim embedTrueTypeFonts = Type.Missing
        Dim saveNativePictureFormat = Type.Missing
        Dim saveFormsData = Type.Missing
        Dim saveAsAOCELetter = Type.Missing
        Dim encoding = Type.Missing
        Dim insertLineBreaks = Type.Missing
        Dim allowSubstitutions = Type.Missing
        Dim lineEnding = Type.Missing
        Dim addBiDiMarks = Type.Missing
        thisDocument.SaveAs(fileName, fileFormat, lockComments, password, addToRecentFiles, writePassword, readOnlyRecommended, embedTrueTypeFonts, saveNativePictureFormat, saveFormsData, saveAsAOCELetter, encoding, insertLineBreaks, allowSubstitutions, lineEnding, addBiDiMarks)
    End Sub
    ''' <summary>
    ''' Возвращает полное имя текущего файла
    ''' </summary>
    ''' <returns>Строка с именем</returns>
    ''' <remarks></remarks>
    Public Function GetDocFName() As String
        Return thisDocument.FullName
    End Function
    ''' <summary>
    ''' Проверяет сохранился ли документ
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckSaved() As Boolean
        If thisDocument.Saved = Boolean.TrueString Then
            MsgBox("Документ сохранен!", MsgBoxStyle.OkOnly, thisDocument.FullName)
        Else
            MsgBox("Документ НЕ сохранен!", MsgBoxStyle.OkOnly, thisDocument.FullName)
        End If
        Return thisDocument.Saved
    End Function
    ''' <summary>
    ''' Закрывает текущий документ
    ''' </summary>
    ''' <remarks>Для открытия пользуемся OpenDoc()</remarks>
    Public Sub CloseDoc()
        Dim saveChanges = -1 'wdSaveChanges
        Dim originalFormat = Type.Missing
        Dim routeDocument = Type.Missing
        thisDocument.Close(saveChanges, originalFormat, routeDocument)
        docOpened = False
    End Sub
    ''' <summary>
    ''' Закрывает текущий документ без сохранения
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseDocNoSave()
        Dim saveChanges = 0 'wdDoNotSaveChanges
        Dim originalFormat = Type.Missing
        Dim routeDocument = Type.Missing
        thisDocument.Close(saveChanges, originalFormat, routeDocument)
    End Sub
    ''' <summary>
    ''' Закрывает текущий документ и сохраняет его
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseDocSave()
        Dim saveChanges = -1 'wdSaveChanges
        Dim originalFormat = Type.Missing
        Dim routeDocument = Type.Missing
        thisDocument.Close(saveChanges, originalFormat, routeDocument)
    End Sub
    ''' <summary>
    ''' Добавить текст в текущий документ
    ''' </summary>
    ''' <param name="str">Текст с переводами и спец знаками</param>
    ''' <remarks>В конце ставит параграф</remarks>
    Public Sub AddText(ByVal str As String)
        Dim sln = thisApplication.Selection
        thisApplication.Options.Overtype = False
        If sln.Type = 1 Then 'wdSelectionIP
            sln.TypeText(str)
            sln.TypeParagraph()
        ElseIf sln.Type = 2 Then 'wdSelectionNormal
            Dim direction = 0 'wdCollapseEnd
            sln.Collapse(direction)
            sln.TypeText(str)
            sln.TypeParagraph()
        Else
            sln.TypeText(str)
            sln.TypeParagraph()
        End If
    End Sub
    ''' <summary>
    ''' Форматировать текст
    ''' </summary>
    ''' <param name="formatFrom">Начальная позиция</param>
    ''' <param name="formatTo">Конечная позиция</param>
    ''' <param name="center">По центру</param>
    ''' <param name="bd">Жирным</param>
    ''' <param name="it">Косой</param>
    ''' <param name="und">Подчеркнутый</param>
    ''' <remarks></remarks>
    Public Sub FormatText(ByVal formatFrom, ByVal formatTo, ByVal center, ByVal bd, ByVal it, ByVal und)
        Dim startpos = formatFrom
        Dim endpos = formatTo
        Dim rng = thisDocument.Range(startpos, endpos)
        rng.Bold = Convert.ToInt32(bd)
        rng.Italic = Convert.ToInt32(it)
        If und = True Then
            rng.Underline = 1 'wdUnderlineSingle
        Else
            rng.Underline = 0 'wdUnderlineNone
        End If
        If center = True Then
            rng.ParagraphFormat.Alignment = 1 'wdAlignParagraphCenter
        Else
            rng.ParagraphFormat.Alignment = 4 'wdAlignParagraphDistribute
        End If
    End Sub
End Class
