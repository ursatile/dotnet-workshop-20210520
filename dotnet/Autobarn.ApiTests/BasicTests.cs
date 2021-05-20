using Shouldly;
using Xunit;


namespace Autobarn.ApiTests {
	public class BasicTests {
		[Fact]
		public void TestsWork() {
			Assert.True(1 == 1);
			(1 == 1).ShouldBe(true);
		}

}
}