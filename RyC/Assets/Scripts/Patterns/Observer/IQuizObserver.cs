public interface IQuizObserver
{
  void OnQuestionLoaded(string questionText, string[] answers, int portalId);
  void OnAnswerCorrect(PlayerIndex player, int portalId);
  void OnAnswerWrong(PlayerIndex player, int portalId);
  void OnQuizFinished();
  void OnRaceFinished(PlayerIndex winner);
}