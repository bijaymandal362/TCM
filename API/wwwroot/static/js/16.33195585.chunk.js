(this.webpackJsonptcm=this.webpackJsonptcm||[]).push([[16],{426:function(e,t,r){"use strict";r.d(t,"c",(function(){return l})),r.d(t,"a",(function(){return i})),r.d(t,"b",(function(){return j}));var a=r(34),c=r.n(a),n=r(59),s=r(55),u=r(43),l=function(){var e=Object(n.a)(c.a.mark((function e(t){var r;return c.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,e.next=4,Object(s.a)({url:"/Project/GetProjectListFilterByModule",method:"POST",params:t});case 4:return r=e.sent,e.abrupt("return",[r,null]);case 8:return e.prev=8,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 11:case"end":return e.stop()}}),e,null,[[1,8]])})));return function(t){return e.apply(this,arguments)}}(),i=function(){var e=Object(n.a)(c.a.mark((function e(t){var r;return c.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,e.next=4,Object(s.a)({url:"/Project/AddProject",method:"POST",data:t});case 4:return r=e.sent,e.abrupt("return",[r,null]);case 8:return e.prev=8,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 11:case"end":return e.stop()}}),e,null,[[1,8]])})));return function(t){return e.apply(this,arguments)}}(),j=function(){var e=Object(n.a)(c.a.mark((function e(t){var r;return c.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,e.next=4,Object(s.a)({url:"/Common/GetListItemByListItemCategorySystemName/".concat(t),method:"GET"});case 4:return r=e.sent,e.abrupt("return",[r,null]);case 8:return e.prev=8,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 11:case"end":return e.stop()}}),e,null,[[1,8]])})));return function(t){return e.apply(this,arguments)}}()},480:function(e,t,r){"use strict";r.r(t);var a=r(34),c=r.n(a),n=r(59),s=r(50),u=r(457),l=r(492),i=r(216),j=r(442),o=r(443),b=r(451),d=r(459),p=r(406),m=r(490),h=r(73),O=r(496),x=r(467),f=r.n(x),v=r(16),g=r(426),k=r(433),w=r.n(k),y=r(0),N=r(4),I=u.a.Option;t.default=function(){var e=l.a.useForm(),t=Object(s.a)(e,1)[0],r=Object(y.useState)([]),a=Object(s.a)(r,2),x=a[0],k=a[1],P=Object(y.useState)(!1),C=Object(s.a)(P,2),S=C[0],M=C[1],D=Object(v.g)(),q=function(){var e=Object(n.a)(c.a.mark((function e(t){var r,a,n,u,l;return c.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return M(!0),r={projectName:t.project_name,startDate:w()(t.start_date).format("YYYY-MM-DD"),projectMarketListItemId:t.market,projectDescription:t.project_description},e.next=4,Object(g.a)(r);case 4:a=e.sent,n=Object(s.a)(a,2),u=n[0],l=n[1],M(!1),u&&(i.b.success("Projet created Sucessfully "),D.push("/")),l&&i.b.error("Unable to create project !");case 11:case"end":return e.stop()}}),e)})));return function(t){return e.apply(this,arguments)}}(),_=function(){var e=Object(n.a)(c.a.mark((function e(){var t,r,a,n;return c.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return e.next=2,Object(g.b)("ProjectMarket");case 2:t=e.sent,r=Object(s.a)(t,2),a=r[0],n=r[1],a&&k(a.data),n&&i.b.error("Failed to list market");case 8:case"end":return e.stop()}}),e)})));return function(){return e.apply(this,arguments)}}();return Object(y.useEffect)((function(){_()}),[]),Object(N.jsx)("div",{children:Object(N.jsxs)(j.a,{className:"mt-4 p-4",children:[Object(N.jsxs)(o.a,{lg:6,xs:24,className:"mx-auto",children:[Object(N.jsx)(O.a,{style:{fontSize:150}}),Object(N.jsx)("h4",{children:"Create Blank Project"}),Object(N.jsx)("p",{children:"Create a blank project to house your files, plan your work, and collaborate on code, among other things."})]}),Object(N.jsxs)(o.a,{lg:14,xs:24,className:"p-2 mt-2",children:[Object(N.jsx)(j.a,{children:Object(N.jsxs)(o.a,{lg:18,xs:24,children:[Object(N.jsxs)(b.a,{children:[Object(N.jsx)(b.a.Item,{children:"New Project"}),Object(N.jsx)(b.a.Item,{children:Object(N.jsx)("a",{href:"",children:"Create Blank Project"})})]}),Object(N.jsx)(d.a,{})]})}),Object(N.jsxs)(l.a,{className:"mt-4",layout:"vertical",onFinish:q,form:t,children:[Object(N.jsx)(j.a,{gutter:24,children:Object(N.jsx)(o.a,{lg:16,xs:24,className:"gutter-row",children:Object(N.jsx)(l.a.Item,{label:"Project Name",name:"project_name",rules:[{required:!0,message:"Project Name is Required"}],children:Object(N.jsx)(p.a,{placeholder:"My awesome project"})})})}),Object(N.jsxs)(j.a,{gutter:24,children:[Object(N.jsx)(o.a,{lg:8,xs:24,className:"gutter-row",children:Object(N.jsx)(l.a.Item,{label:"Start Date",name:"start_date",rules:[{required:!0,message:"Start Date is Required"}],children:Object(N.jsx)(m.a,{className:"w-100",onChange:function(){}})})}),Object(N.jsx)(o.a,{lg:8,xs:24,className:"gutter-row",children:Object(N.jsx)(l.a.Item,{label:"Market",name:"market",rules:[{required:!0,message:"Market is Required"}],children:Object(N.jsx)(u.a,{placeholder:"Select Market",children:x.map((function(e,t){return Object(N.jsx)(I,{value:null===e||void 0===e?void 0:e.listItemId,children:null===e||void 0===e?void 0:e.listItemName},t)}))})})})]}),Object(N.jsx)(j.a,{children:Object(N.jsx)(o.a,{lg:16,xs:24,children:Object(N.jsx)(l.a.Item,{label:"Project Description (Optional)",name:"project_description",children:Object(N.jsx)(f.a,{showCount:!0,maxLength:100,onChange:function(){}})})})}),Object(N.jsxs)(j.a,{className:"mt-4",children:[Object(N.jsx)(o.a,{lg:8,xs:12,children:Object(N.jsx)(l.a.Item,{children:Object(N.jsx)(h.a,{loading:S,type:"primary",htmlType:"submit",children:"Create Project"})})}),Object(N.jsx)(o.a,{lg:8,xs:12,className:"d-flex justify-content-end",children:Object(N.jsx)(l.a.Item,{children:Object(N.jsx)(h.a,{type:"default",onClick:function(){D.push("/")},children:"Cancel"})})})]})]})]})]})})}}}]);
//# sourceMappingURL=16.33195585.chunk.js.map