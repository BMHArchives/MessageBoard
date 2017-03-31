// home-index.js
var homeIndexModule = angular.module("homeIndex", ['ngRoute'])
    .controller('topicsController', topicsController)
    .controller('newTopicController', newTopicController)
    .controller('singleTopicController', singleTopicController);
//page-centric routes
homeIndexModule.config(function ($routeProvider) {
    $routeProvider.when(
        "/",
        {
            controller: "topicsController",
            templateUrl: "/templates/topicsView.html"
        });
    $routeProvider.when(
        "/newmessage",
        {
            controller: "newTopicController",
            templateUrl: "/templates/newTopicView.html"
        });
    $routeProvider.when(
        "/message/:id",
        {
            controller: "singleTopicController",
            templateUrl: "/templates/singleTopicView.html"
        });
    $routeProvider.otherwise({ redirectTo: "/" });
    // configure html5 to get links working on jsfiddle

});

homeIndexModule.factory("dataService", ["$http", "$q", function ($http, $q) {
    var _topics = [];
    var _isInit = false;
    var _isReady = function () {
        return _isInit;
    }
    var _getTopics = function () {

        var deferred = $q.defer();

        $http.get("/api/v1/topics?includeReplies=true")
        .then(function (result) {
            // Successful
            angular.copy(result.data, _topics);
            _isInit = true;
            deferred.resolve();
        },
        function () {
            deferred.reject();
        });
        return deferred.promise;
    };
    var _addTopic = function (newTopic) {
        var deferred = $q.defer();
        $http.post("/api/v1/topics", newTopic)
        .then(function (result) {
            //success
            var newlyCreatedTopic = result.data;
            _topics.splice(0, 0, newlyCreatedTopic); //This will merge with the existing list of topics
            deferred.resolve(newlyCreatedTopic);
        }, function () {
            //error
            deferred.reject();
        });
        return deferred.promise;
    };

    var _getTopicById = function (id) {
        var deferred = $q.defer();
        //If the data is ready
        if (_isReady()) {
            var topic = _findTopic(id);
            if (topic) {
                deferred.resolve(topic);
            }
            else
            {
                deferred.reject();
            }
        }
        else
        {
            _getTopics().then(function () {
                var topic = _findTopic(id);
                if (topic) {
                    deferred.resolve(topic);
                }
                else {
                    deferred.reject();
                }
            },
            function () {
                deferred.reject();
            });
        }
        return deferred.promise;
    };
    function _findTopic(id) {
        var found = null;
        $.each(_topics, function (i, item) {
            if (item.id == id) {
                found = item;
                return false;
            }
        });
        return found;
    }
    var _saveReply = function(topic, newReply){
        var deferred = $q.defer();

        $http.post("/api/v1/topics/"+topic.id+"/replies", newReply)
        .then(function(result){
            //success
            if(topic.replies == null) topic.replies = [];
            topic.replies.push(result.data);
            deferred.resolve(result.data);
        },function(){
            //errr
            deferred.reject();
        });
        return deferred.promise;
    };
    return {
        topics: _topics,
        getTopics: _getTopics,
        addTopic: _addTopic,
        isReady: _isReady,
        getTopicById: _getTopicById,
        saveReply: _saveReply
    };
}]);
function topicsController($scope, $http, dataService) {

    $scope.data = dataService;
    $scope.isBusy = false;
    if (dataService.isReady() == false) {

        $scope.isBusy = true;
        dataService.getTopics()
            .then(function () {
                //success
            },
            function () {
                //error
                alert("could not load topics")
            }).finally(function () {
                $scope.isBusy = false;
            });
    }
}

topicsController.$inject = ['$scope', '$http', 'dataService'];
newTopicController.$inject = ['$scope', '$http', '$window', 'dataService'];
singleTopicController.$inject = ['$scope', 'dataService', '$window','$routeParams' ];
function newTopicController($scope, $http, $window, dataService) {
    $scope.newTopic = {};
    $scope.save = function () {
        dataService.addTopic($scope.newTopic).
        then(function () {
            //success
            $window.location = "#/";
        },
        function () {
            //error
            alert("could not save the new topic");
        });
    }
}
function singleTopicController($scope, dataService, $window, $routeParams) {
    $scope.topic = null;
    $scope.newReply = {};

    dataService.getTopicById($routeParams.id)
    .then(function (topic) {
        //success
        $scope.topic = topic;
    },
    function () {
        //error
        $window.location = "#/";
    });
    $scope.addReply = function () {
        dataService.saveReply($scope.topic, $scope.newReply)
        .then(function () {
            //success
            $scope.newReply.body = "";
        },
        function () {
            //error
        });
    };
}
//angular.module('homeIndex', []).controller('topicsController', topicsController);