using Nest;

namespace UserService {
   public class ElasticsearchConfig {
      public static ElasticClient CreateClient() {
         var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200"))
             .DefaultIndex("customers");

         var client = new ElasticClient(settings);
         return client;
      }

   }
}
