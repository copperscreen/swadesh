<script lang="ts">
export default {
  data() {
    return {
      list: null,
      checked: []
    }
  },
  mounted() {
     fetch( '/list' ).then( result => result.json().then( json => this.list = json ));
  },
  methods: {
     selectall(ev){
        this.checked.splice.apply(this.checked, [0, this.checked.length].concat(this.list.map(_ => _[0])) );
     },
     selectnone(ev){
        this.checked.splice(0);
     },
      compile(ev) {
          let checked = Array.from(this.checked);
          if (checked.length) {
              this.$router.push({ name: 'list', query: { l: Array.from(this.checked) } });
          } else {
              alert('Select languages');
          }
     }
  }
}
</script>

<template>
  <div class="list" v-if=list>
    <input type="button" value="Select all" @click="selectall" />
    <input type="button" value="Unselect all" @click="selectnone"/>
    <input type="button" value="Compile" @click="compile"/>
    <div class="lang-list">
      <span v-for="item in list">
        <input type="checkbox" v-model="checked" v-bind:name="item[0]" v-bind:id="item[0]" v-bind:value="item[0]" />
        &nbsp;
        <label v-bind:for="item[0]">{{item[1]}}</label>
      </span>
    </div>
    <input type="button" value="Select all" @click="selectall"/>
    <input type="button" value="Unselect all" @click="selectnone"/>
    <input type="button" value="Compile" @click="compile"/>
  </div>
  <div class="waiting" v-else>
Loading
  </div>
</template>

<style scoped>
.lang-list{
  display: grid;
}
@media (min-width: 500px) { .lang-list{  grid-template-columns: repeat(2, 1fr); } }
@media (min-width: 700px) { .lang-list{  grid-template-columns: repeat(3, 1fr); } }
@media (min-width: 900px) { .lang-list{  grid-template-columns: repeat(4, 1fr); } }
</style>
