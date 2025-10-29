namespace InbentorySystem.Services
{
    /// <summary>
    /// コンポーネントの便利機能をまとめたユーティリティクラス
    /// </summary>
    public static class ComponentUtils
    {

        /// <summary>
        /// string.Emptyを参照渡しすることでフォーム、エラーメッセージをリセットする
        /// </summary>
        public static void ResetSearchForm(ref string keyword, ref string errorMessage)
        {
            keyword = string.Empty;
            errorMessage = string.Empty;
        }
    }
}

